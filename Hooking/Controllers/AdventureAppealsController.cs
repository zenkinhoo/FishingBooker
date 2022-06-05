using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;

namespace Hooking.Controllers
{
    public class AdventureAppealsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public AdventureAppealsController(ApplicationDbContext context, 
            IEmailSender emailSender, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;

            using StreamReader reader = new StreamReader("./Data/emailCredentials.json");
            string json = reader.ReadToEnd();
            _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
        }

        // GET: AdventureAppeals
        public async Task<IActionResult> Index()
        {
            /*_context.Add(new AdventureAppeal
            {
                AdventureId = "713c6ee8-7a7a-4116-b020-2ea29ddc91d6",
                AppealContent = "Zaaaaalbaaa",
                UserEmail = "sykohoto@onekisspresave.com"
            });*/
            _context.SaveChanges();

            return View(await _context.AdventureAppeal.ToListAsync());
        }

        public IActionResult AnswerAppeal(Guid id)
        {
            AdventureAppeal appeal = _context.AdventureAppeal.Find(id);
            return View(appeal);
        }

        private string GetInstructorEmailFromAppeal(AdventureAppeal appeal)
        {
            Adventure adventure = _context.Adventure.Find(Guid.Parse(appeal.AdventureId));
            Instructor instructor = _context.Instructor.Find(Guid.Parse(adventure.InstructorId));
            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(instructor.UserDetailsId));
            return _userManager.FindByIdAsync(userDetails.IdentityUserId).WaitAndUnwrapException().Email;
        }

        public async Task<IActionResult> SubmitAnswer([Bind("AdventureId,AppealContent,UserEmail,Id,RowVersion")] AdventureAppeal appeal, string answer)
        {
            await _emailSender.SendEmailAsync(appeal.UserEmail, "Odgovor na žalbu", answer);
            string instructorEmail = GetInstructorEmailFromAppeal(appeal);
            await _emailSender.SendEmailAsync(instructorEmail, "Odgovor na žalbu", answer);
            appeal = _context.AdventureAppeal.FirstOrDefault(a => a.Id == appeal.Id);
            if (appeal == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.AdventureAppeal.Remove(appeal);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AdventureAppeals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureAppeal = await _context.AdventureAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureAppeal == null)
            {
                return NotFound();
            }

            return View(adventureAppeal);
        }

        private async Task<bool> adventureAppealExists(AdventureAppeal adventureAppeal, string adventureId)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (AdventureAppeal advAppeal in _context.AdventureAppeal.ToList())
            {
                var appealUser = _context.Users.Where(m => m.Email == adventureAppeal.UserEmail);
                if (user.Email == adventureAppeal.UserEmail && adventureAppeal.AdventureId == adventureId)
                    return true;
            }

            return false;
        }

        // GET: AdventureAppeals/Create
        public IActionResult Create(Guid id, String instructorId)
        {

            Adventure adv = _context.Adventure.Where(m => m.Id == id).FirstOrDefault();
            UserDetails userInstructor = _context.UserDetails.Where(m => m.Id == Guid.Parse(instructorId)).FirstOrDefault();

            ViewData["Adventure"] = adv;
            ViewData["UserInstructor"] = userInstructor;
            return View();
        }

        // POST: AdventureAppeals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/AdventureAppeals/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("AdventureId,AppealContent,Id,RowVersion")] AdventureAppeal adventureAppeal)
        {
            if (ModelState.IsValid)
            {
                adventureAppeal.Id = Guid.NewGuid();
                adventureAppeal.AdventureId = id.ToString();
                var user = await _userManager.GetUserAsync(User);
                adventureAppeal.UserEmail = user.Email;
                if (!(await adventureAppealExists(adventureAppeal, adventureAppeal.AdventureId)))
                {
                    _context.Add(adventureAppeal);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index","Instructors");
            }
            return View(adventureAppeal);
        }

        // GET: AdventureAppeals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureAppeal = await _context.AdventureAppeal.FindAsync(id);
            if (adventureAppeal == null)
            {
                return NotFound();
            }
            return View(adventureAppeal);
        }

        // POST: AdventureAppeals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AdventureId,AppealContent,Id,RowVersion")] AdventureAppeal adventureAppeal)
        {
            if (id != adventureAppeal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureAppeal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureAppealExists(adventureAppeal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adventureAppeal);
        }

        // GET: AdventureAppeals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureAppeal = await _context.AdventureAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureAppeal == null)
            {
                return NotFound();
            }

            return View(adventureAppeal);
        }

        // POST: AdventureAppeals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureAppeal = await _context.AdventureAppeal.FindAsync(id);
            _context.AdventureAppeal.Remove(adventureAppeal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureAppealExists(Guid id)
        {
            return _context.AdventureAppeal.Any(e => e.Id == id);
        }
    }
}
