using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Nito.AsyncEx.Synchronous;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.IO;

namespace Hooking.Controllers
{
    public class CottageAppealsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CottageAppealsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, 
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;

            using StreamReader reader = new StreamReader("./Data/emailCredentials.json");
            string json = reader.ReadToEnd();
            _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
        }

        // GET: CottageAppeals
        public async Task<IActionResult> Index()
        {


            return View(await _context.CottageAppeal.ToListAsync());
        }

        public IActionResult AnswerAppeal(Guid id)
        {
            CottageAppeal appeal = _context.CottageAppeal.Find(id);
            return View(appeal);
        }

        private string GetCottageOwnerEmailFromAppeal(CottageAppeal appeal)
        {
            Cottage cottage = _context.Cottage.Find(Guid.Parse(appeal.CottageId));
            CottageOwner owner = _context.CottageOwner.Find(Guid.Parse(cottage.CottageOwnerId));
            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(owner.UserDetailsId));
            return _userManager.FindByIdAsync(userDetails.IdentityUserId).WaitAndUnwrapException().Email;
        }

        public async Task<IActionResult> SubmitAnswer([Bind("CottageId,AppealContent,UserEmail,Id,RowVersion")] CottageAppeal appeal, string answer)
        {
            await _emailSender.SendEmailAsync(appeal.UserEmail, "Odgovor na žalbu", answer);
            string ownerEmail = GetCottageOwnerEmailFromAppeal(appeal);
            await _emailSender.SendEmailAsync(ownerEmail, "Odgovor na žalbu", answer);
            appeal = _context.CottageAppeal.FirstOrDefault(a => a.Id == appeal.Id);
            if (appeal == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.CottageAppeal.Remove(appeal);
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

        // GET: CottageAppeals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageAppeal = await _context.CottageAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageAppeal == null)
            {
                return NotFound();
            }

            return View(cottageAppeal);
        }

        private async Task<bool> cottageAppealExists(CottageAppeal cottageAppeal,string cottageId)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (CottageAppeal ctgAppeal in _context.CottageAppeal.ToList())
            {
                var appealUser = _context.Users.Where(m => m.Email == ctgAppeal.UserEmail);
                if (user.Email == ctgAppeal.UserEmail && ctgAppeal.CottageId == cottageId)
                    return true;
            }

            return false;
        }

        // GET: CottageAppeals/Create
        public IActionResult Create(Guid id, String cottageOwnerId)
        {
            Cottage ctg = _context.Cottage.Where(m => m.Id == id).FirstOrDefault();
            CottageOwner ctgOwner = _context.CottageOwner.Where(m => m.Id == Guid.Parse(cottageOwnerId)).FirstOrDefault();
            UserDetails cottageOwnerUser = _context.UserDetails.Where(m => m.Id == Guid.Parse(ctgOwner.UserDetailsId)).FirstOrDefault();

            ViewData["Cottage"] = ctg;
            ViewData["CottageOwner"] = cottageOwnerUser;
            return View();
        }

        // POST: CottageAppeals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/CottageAppeals/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("CottageId,AppealContent,Id,RowVersion")] CottageAppeal cottageAppeal)
        {
          

            if (ModelState.IsValid)
            {
                cottageAppeal.Id = Guid.NewGuid();
                cottageAppeal.CottageId = id.ToString();
                if (cottageAppeal.UserEmail==null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    cottageAppeal.UserEmail = user.Email;
                }
                if(!(await cottageAppealExists(cottageAppeal, cottageAppeal.CottageId)))
                {
                    _context.Add(cottageAppeal);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Cottages");
            }
            return View(cottageAppeal);
        }

        // GET: CottageAppeals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageAppeal = await _context.CottageAppeal.FindAsync(id);
            if (cottageAppeal == null)
            {
                return NotFound();
            }
            return View(cottageAppeal);
        }

        // POST: CottageAppeals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,AppealContent,Id,RowVersion")] CottageAppeal cottageAppeal)
        {
            if (id != cottageAppeal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageAppeal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageAppealExists(cottageAppeal.Id))
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
            return View(cottageAppeal);
        }

        // GET: CottageAppeals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageAppeal = await _context.CottageAppeal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageAppeal == null)
            {
                return NotFound();
            }

            return View(cottageAppeal);
        }

        // POST: CottageAppeals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageAppeal = await _context.CottageAppeal.FindAsync(id);
            _context.CottageAppeal.Remove(cottageAppeal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageAppealExists(Guid id)
        {
            return _context.CottageAppeal.Any(e => e.Id == id);
        }
    }
}
