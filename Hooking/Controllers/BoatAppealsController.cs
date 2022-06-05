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
    public class BoatAppealsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public BoatAppealsController(ApplicationDbContext context,
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

        // GET: BoatAppeals
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatAppeal.ToListAsync());
        }

        // GET: BoatAppeals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAppeal = await _context.BoatAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatAppeal == null)
            {
                return NotFound();
            }

            return View(boatAppeal);
        }

        public IActionResult AnswerAppeal(Guid id)
        {
            BoatAppeal appeal = _context.BoatAppeal.Find(id);
            return View(appeal);
        }

        private string GetBoatOwnerEmailFromAppeal(BoatAppeal appeal)
        {
            Boat boat = _context.Boat.Find(Guid.Parse(appeal.BoatId));
            BoatOwner owner = _context.BoatOwner.Find(Guid.Parse(boat.BoatOwnerId));
            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(owner.UserDetailsId));
            return _userManager.FindByIdAsync(userDetails.IdentityUserId).WaitAndUnwrapException().Email;
        }
        public async Task<IActionResult> SubmitAnswer([Bind("BoatId,AppealContent,UserEmail,Id,RowVersion")] BoatAppeal appeal, string answer)
        {
            await _emailSender.SendEmailAsync(appeal.UserEmail, "Odgovor na žalbu", answer);
            string boatOwnerEmail = GetBoatOwnerEmailFromAppeal(appeal);
            await _emailSender.SendEmailAsync(boatOwnerEmail, "Odgovor na žalbu", answer);
            appeal = _context.BoatAppeal.FirstOrDefault(a => a.Id == appeal.Id);
            if (appeal == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.BoatAppeal.Remove(appeal);
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

        private async Task<bool> boatAppealExists(BoatAppeal boatAppeal, string boatId)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (BoatAppeal btAppeal in _context.BoatAppeal.ToList())
            {
                var appealUser = _context.Users.Where(m => m.Email == btAppeal.UserEmail);
                if (user.Email == btAppeal.UserEmail && btAppeal.BoatId == boatId)
                    return true;
            }

            return false;
        }

        // GET: BoatAppeals/Create
        public IActionResult Create(Guid id, String boatOwnerId)
        {

            Boat bt = _context.Boat.Where(m => m.Id == id).FirstOrDefault();
            BoatOwner btOwner = _context.BoatOwner.Where(m => m.Id == Guid.Parse(boatOwnerId)).FirstOrDefault();
            UserDetails boatOwnerUser = _context.UserDetails.Where(m => m.Id == Guid.Parse(btOwner.UserDetailsId)).FirstOrDefault();

            ViewData["Boat"] = bt;
            ViewData["BoatOwner"] = boatOwnerUser;
            return View();
        }

        // POST: BoatAppeals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/BoatAppeals/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("BoatId,AppealContent,Id,RowVersion")] BoatAppeal boatAppeal)
        {
            if (ModelState.IsValid)
            {
                boatAppeal.Id = Guid.NewGuid();
                boatAppeal.BoatId = id.ToString();
                var user = await _userManager.GetUserAsync(User);
                boatAppeal.UserEmail = user.Email;
                if (!(await boatAppealExists(boatAppeal, boatAppeal.BoatId)))
                {
                    _context.Add(boatAppeal);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Boats");

               // return RedirectToAction(nameof(Index));
            }
            return View(boatAppeal);
        }

        // GET: BoatAppeals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAppeal = await _context.BoatAppeal.FindAsync(id);
            if (boatAppeal == null)
            {
                return NotFound();
            }
            return View(boatAppeal);
        }

        // POST: BoatAppeals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,AppealContent,Id,RowVersion")] BoatAppeal boatAppeal)
        {
            if (id != boatAppeal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatAppeal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatAppealExists(boatAppeal.Id))
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
            return View(boatAppeal);
        }

        // GET: BoatAppeals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAppeal = await _context.BoatAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatAppeal == null)
            {
                return NotFound();
            }

            return View(boatAppeal);
        }

        // POST: BoatAppeals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatAppeal = await _context.BoatAppeal.FindAsync(id);
            _context.BoatAppeal.Remove(boatAppeal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatAppealExists(Guid id)
        {
            return _context.BoatAppeal.Any(e => e.Id == id);
        }
    }
}
