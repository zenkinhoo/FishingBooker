using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;

namespace Hooking.Controllers
{
    public class AdventureReservationReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventureReservationReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventureReservationReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureReservationReview.ToListAsync());
        }

        // GET: AdventureReservationReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationReview = await _context.AdventureReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReservationReview == null)
            {
                return NotFound();
            }

            return View(adventureReservationReview);
        }

        // GET: AdventureReservationReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureReservationReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/AdventureReservationReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("Review,DidntShow,ReceivedPenalty,Id,RowVersion")] AdventureReservationReview adventureReservationReview)
        {
            if (ModelState.IsValid)
            {
                AdventureReservation adventureReservation = await _context.AdventureReservation.Where(m => m.Id == id).FirstOrDefaultAsync<AdventureReservation>();
                adventureReservation.IsReviewed = true;
                await _context.SaveChangesAsync();
                adventureReservationReview.Id = Guid.NewGuid();
                adventureReservationReview.AdventureReservationId = adventureReservation.Id.ToString();
                if (!adventureReservationReview.DidntShow)
                {
                    Guid userId = Guid.Parse(adventureReservation.UserDetailsId);
                    UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                    userDetails.PenaltyCount++;
                    _context.Update(userDetails);
                    await _context.SaveChangesAsync();

                }
                if (adventureReservationReview.ReceivedPenalty)
                {
                    adventureReservationReview.IsReviewedByAdmin = false;
                }
                else
                {
                    adventureReservationReview.IsReviewedByAdmin = true;
                }
                _context.Add(adventureReservationReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureReservationReview);
        }

        // GET: AdventureReservationReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationReview = await _context.AdventureReservationReview.FindAsync(id);
            if (adventureReservationReview == null)
            {
                return NotFound();
            }
            return View(adventureReservationReview);
        }

        // POST: AdventureReservationReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReceivedPenalty,Id,RowVersion")] AdventureReservationReview adventureReservationReview)
        {
            if (id != adventureReservationReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var temp = _context.AdventureReservationReview.Find(id);
                    temp.ReceivedPenalty = adventureReservationReview.ReceivedPenalty;
                    temp.IsReviewedByAdmin = true;
                    _context.Update(temp);
                    await _context.SaveChangesAsync();
                    if (adventureReservationReview.ReceivedPenalty)
                    {
                        var adventureReservation = _context.AdventureReservation.Find(Guid.Parse(temp.AdventureReservationId));
                        Guid userId = Guid.Parse(adventureReservation.UserDetailsId);
                        UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                        userDetails.PenaltyCount++;
                        _context.Update(userDetails);
                        await _context.SaveChangesAsync();

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureReservationReviewExists(adventureReservationReview.Id))
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
            return View(adventureReservationReview);
        }

        // GET: AdventureReservationReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationReview = await _context.AdventureReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReservationReview == null)
            {
                return NotFound();
            }

            return View(adventureReservationReview);
        }

        // POST: AdventureReservationReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureReservationReview = await _context.AdventureReservationReview.FindAsync(id);
            _context.AdventureReservationReview.Remove(adventureReservationReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureReservationReviewExists(Guid id)
        {
            return _context.AdventureReservationReview.Any(e => e.Id == id);
        }
    }
}
