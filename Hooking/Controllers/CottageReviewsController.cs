using System;
using System.Collections.Generic;
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

namespace Hooking.Controllers
{
    public class CottageReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CottageReviewsController(ApplicationDbContext context, 
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

        // GET: CottageReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageReview.ToListAsync());
        }

        public async Task<IActionResult> Approve(Guid id)
        {
            CottageReview review = await _context.CottageReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsReviewed = true;
            review.IsApproved = true;

            Cottage cottage = _context.Cottage.Find(Guid.Parse(review.CottageId));

            CottageOwner owner = _context.CottageOwner.Find(Guid.Parse(cottage.CottageOwnerId));

            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(owner.UserDetailsId));

            IdentityUser iUser = await _userManager.FindByIdAsync(userDetails.IdentityUserId);

            await _emailSender.SendEmailAsync(iUser.Email, "Odobrena revizija",
                $"Revizija sa sadržajem '{review.Review}' i ocenom {review.Grade} je podneta za Vas.");

            await _context.SaveChangesAsync();

            List<CottageReview> ctgReviews = _context.CottageReview.Where(m => m.CottageId == review.CottageId).ToList();

            Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(review.CottageId)).FirstOrDefault();
            int gradeCount = 0;
            double gradeSum = 0;
            foreach (CottageReview ctgReview in ctgReviews)
            {
                if (ctg.Id == Guid.Parse(ctgReview.CottageId) && ctgReview.IsApproved)
                {
                    gradeCount++;
                    gradeSum += Convert.ToDouble(ctgReview.Grade);
                }
            }

            ctg.AverageGrade = Math.Round(gradeSum / gradeCount, 2);
            ctg.GradeCount = gradeCount;
            System.Diagnostics.Debug.WriteLine("grade count je " + gradeCount.ToString());

            _context.Update(ctg);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deny(Guid id)
        {
            CottageReview review = await _context.CottageReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsReviewed = true;
            review.IsApproved = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: CottageReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReview = await _context.CottageReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReview == null)
            {
                return NotFound();
            }

            return View(cottageReview);
        }

        // GET: CottageReviews/Create
        public IActionResult Create(Guid id, String cottageOwnerId)
        {
            Cottage ctg = _context.Cottage.Where(m => m.Id == id).FirstOrDefault();
            CottageOwner ctgOwner = _context.CottageOwner.Where(m => m.Id == Guid.Parse(cottageOwnerId)).FirstOrDefault();
            UserDetails cottageOwnerUser = _context.UserDetails.Where(m => m.Id == Guid.Parse(ctgOwner.UserDetailsId)).FirstOrDefault();

            ViewData["Cottage"] = ctg;
            ViewData["CottageOwner"] = cottageOwnerUser;
            return View();
        }

        // POST: CottageReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/CottageReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("CottageId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] CottageReview cottageReview)
        {
            if (ModelState.IsValid)
            {
                cottageReview.Id = Guid.NewGuid();
                cottageReview.CottageId = id.ToString();
                var user = await _userManager.GetUserAsync(User);
                UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == user.Id);
                cottageReview.UserDetailsId = userDetails.Id.ToString();
                _context.Add(cottageReview);
                await _context.SaveChangesAsync();
                //updateujemo prosecnu ocenu u vikendici



                return RedirectToAction("Index", "Cottages");



            }
            return View(cottageReview);
        }

        // GET: CottageReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReview = await _context.CottageReview.FindAsync(id);
            if (cottageReview == null)
            {
                return NotFound();
            }
            return View(cottageReview);
        }

        // POST: CottageReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] CottageReview cottageReview)
        {
            if (id != cottageReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageReviewExists(cottageReview.Id))
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
            return View(cottageReview);
        }

        // GET: CottageReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReview = await _context.CottageReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReview == null)
            {
                return NotFound();
            }

            return View(cottageReview);
        }

        // POST: CottageReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageReview = await _context.CottageReview.FindAsync(id);
            _context.CottageReview.Remove(cottageReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageReviewExists(Guid id)
        {
            return _context.CottageReview.Any(e => e.Id == id);
        }
    }
}
