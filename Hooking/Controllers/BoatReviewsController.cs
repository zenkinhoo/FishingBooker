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
    public class BoatReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BoatReviewsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, 
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;


        }

        // GET: BoatReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatReview.ToListAsync());
        }

        public async Task<IActionResult> Approve(Guid id)
        {
            BoatReview review = await _context.BoatReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsReviewed = true;
            review.IsApproved = true;

            Boat boat = _context.Boat.Find(Guid.Parse(review.BoatId));

            BoatOwner owner = _context.BoatOwner.Find(Guid.Parse(boat.BoatOwnerId));

            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(owner.UserDetailsId));

            IdentityUser iUser = await _userManager.FindByIdAsync(userDetails.IdentityUserId);

            await _emailSender.SendEmailAsync(iUser.Email, "Odobrena revizija",
                $"Revizija sa sadržajem '{review.Review}' i ocenom {review.Grade} je podneta za Vas.");

            await _context.SaveChangesAsync();


            List<BoatReview> btReviews = _context.BoatReview.Where(m => m.BoatId == review.BoatId).ToList();
            Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(review.BoatId)).FirstOrDefault();
            int gradeCount = 0;
            double gradeSum = 0;
            foreach (BoatReview btReview in btReviews)
            {
                if (bt.Id == Guid.Parse(btReview.BoatId) && btReview.IsApproved)
                {
                    gradeCount++;
                    gradeSum += Convert.ToDouble(btReview.Grade);
                }
            }

            bt.AverageGrade = Math.Round(gradeSum / gradeCount, 2);
            bt.GradeCount = gradeCount;

            _context.Update(bt);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deny(Guid id)
        {
            BoatReview review = await _context.BoatReview.FindAsync(id);
            if (review == null) return NotFound();

            review.IsReviewed = true;
            review.IsApproved = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: BoatReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReview = await _context.BoatReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReview == null)
            {
                return NotFound();
            }

            return View(boatReview);
        }

        // GET: BoatReviews/Create
        public IActionResult Create(Guid id, String boatOwnerId)
        {

            Boat bt = _context.Boat.Where(m => m.Id == id).FirstOrDefault();
            BoatOwner btOwner = _context.BoatOwner.Where(m => m.Id == Guid.Parse(boatOwnerId)).FirstOrDefault();
            UserDetails boatOwnerUser = _context.UserDetails.Where(m => m.Id == Guid.Parse(btOwner.UserDetailsId)).FirstOrDefault();

            ViewData["Boat"] = bt;
            ViewData["BoatOwner"] = boatOwnerUser;
            return View();
        }

        // POST: BoatReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/BoatReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("BoatId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] BoatReview boatReview)
        {
            if (ModelState.IsValid)
            {
                boatReview.Id = Guid.NewGuid();
                boatReview.BoatId = id.ToString();
                var user = await _userManager.GetUserAsync(User);
                UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == user.Id);
                boatReview.UserDetailsId = userDetails.Id.ToString();
                _context.Add(boatReview);
                await _context.SaveChangesAsync();


               



                return RedirectToAction("Index", "Boats");
            }
            return View(boatReview);
        }

        // GET: BoatReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReview = await _context.BoatReview.FindAsync(id);
            if (boatReview == null)
            {
                return NotFound();
            }
            return View(boatReview);
        }

        // POST: BoatReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] BoatReview boatReview)
        {
            if (id != boatReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatReviewExists(boatReview.Id))
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
            return View(boatReview);
        }

        // GET: BoatReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReview = await _context.BoatReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReview == null)
            {
                return NotFound();
            }

            return View(boatReview);
        }

        // POST: BoatReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatReview = await _context.BoatReview.FindAsync(id);
            _context.BoatReview.Remove(boatReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatReviewExists(Guid id)
        {
            return _context.BoatReview.Any(e => e.Id == id);
        }
    }
}
