using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Hooking.Controllers
{
    public class AdventureReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;


        public AdventureReviewsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;

        }

        // GET: AdventureReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureReview.ToListAsync());
        }

        // GET: AdventureReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReview = await _context.AdventureReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReview == null)
            {
                return NotFound();
            }

            return View(adventureReview);
        }
        public async Task<IActionResult> Approve(Guid id)
        {
            AdventureReview review = await _context.AdventureReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsApproved = true;

            Adventure adventure = _context.Adventure.Find(Guid.Parse(review.AdventureId));

            Instructor instructor = _context.Instructor.Find(Guid.Parse(adventure.InstructorId));

            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(instructor.UserDetailsId));

            IdentityUser iUser = await _userManager.FindByIdAsync(userDetails.IdentityUserId);

            await _emailSender.SendEmailAsync(iUser.Email, "Odobrena revizija",
                $"Revizija sa sadržajem '{review.Review}' i ocenom {review.Grade} je podneta za Vas.");

            await _context.SaveChangesAsync();

            List<AdventureReview> advReviews = _context.AdventureReview.Where(m => m.AdventureId == review.AdventureId).ToList();

            //ovde updatujemo prosenu ocenu za avanturu

            Adventure adv = _context.Adventure.Where(m => m.Id == Guid.Parse(review.AdventureId)).FirstOrDefault();
            int gradeCount = 0;
            double gradeSum = 0;
            foreach (AdventureReview advReview in advReviews)
            {
                if (adv.Id == Guid.Parse(advReview.AdventureId) && advReview.IsApproved)
                {
                    gradeCount++;
                    gradeSum += Convert.ToDouble(advReview.Grade);
                }
            }

            adv.AverageGrade = Math.Round(gradeSum / gradeCount, 2);
         //   adv.GradeCount = gradeCount;
            System.Diagnostics.Debug.WriteLine("grade count je " + gradeCount.ToString());

            _context.Update(adv);
            await _context.SaveChangesAsync();

            // ovde updatujemo prosecnu ocenu za instruktora
             gradeCount = 0;
             gradeSum = 0;
            foreach (AdventureReview advReview in advReviews)
            {
                foreach(Adventure ad in _context.Adventure.ToList())
                
                if(ad.Id==Guid.Parse(advReview.AdventureId) && ad.InstructorId==instructor.Id.ToString() 
                        && advReview.IsApproved)
                    {
                        gradeCount++;
                        gradeSum += Convert.ToDouble(advReview.Grade);
                    }
            }
            instructor.AverageGrade = Math.Round(gradeSum / gradeCount, 2);
            instructor.GradeCount = gradeCount;
            System.Diagnostics.Debug.WriteLine("grade count je " + gradeCount.ToString());

            _context.Update(instructor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deny(Guid id)
        {
            AdventureReview review = await _context.AdventureReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsApproved = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: AdventureReviews/Create
        public IActionResult Create(Guid id, String instructorId)
        {

            Adventure adv = _context.Adventure.Where(m => m.Id == id).FirstOrDefault();
            UserDetails userInstructor = _context.UserDetails.Where(m => m.Id == Guid.Parse(instructorId)).FirstOrDefault();

            ViewData["Adventure"] = adv;
            ViewData["UserInstructor"] = userInstructor;
            return View();
        }

        // POST: AdventureReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/AdventureReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("AdventureId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] AdventureReview adventureReview)
        {
            if (ModelState.IsValid)
            {
                adventureReview.Id = Guid.NewGuid();
                adventureReview.AdventureId = id.ToString();
                var user = await _userManager.GetUserAsync(User);
                adventureReview.UserDetailsId = user.Id.ToString();
                _context.Add(adventureReview);
                await _context.SaveChangesAsync();

                List<AdventureReview> advReviews = _context.AdventureReview.Where(m => m.AdventureId == adventureReview.AdventureId).ToList();
                Adventure adv = _context.Adventure.Where(m => m.Id == Guid.Parse(adventureReview.AdventureId)).FirstOrDefault();
                int gradeCount = 0;
                double gradeSum = 0;
                foreach (AdventureReview advReview in advReviews)
                {
                    if (adv.Id == Guid.Parse(advReview.AdventureId))
                    {
                        gradeCount++;
                        gradeSum += Convert.ToDouble(advReview.Grade);
                    }
                }

                adv.AverageGrade = Math.Round(gradeSum / gradeCount, 2);

                _context.Update(adv);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Instructors");

            }
            return View(adventureReview);
        }

        // GET: AdventureReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReview = await _context.AdventureReview.FindAsync(id);
            if (adventureReview == null)
            {
                return NotFound();
            }
            return View(adventureReview);
        }

        // POST: AdventureReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AdventureId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] AdventureReview adventureReview)
        {
            if (id != adventureReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureReviewExists(adventureReview.Id))
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
            return View(adventureReview);
        }

        // GET: AdventureReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReview = await _context.AdventureReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReview == null)
            {
                return NotFound();
            }

            return View(adventureReview);
        }

        // POST: AdventureReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureReview = await _context.AdventureReview.FindAsync(id);
            _context.AdventureReview.Remove(adventureReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureReviewExists(Guid id)
        {
            return _context.AdventureReview.Any(e => e.Id == id);
        }
    }
}
