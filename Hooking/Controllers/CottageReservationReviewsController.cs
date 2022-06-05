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
    public class CottageReservationReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottageReservationReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottageReservationReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageReservationReview.ToListAsync());
        }

        // GET: CottageReservationReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationReview = await _context.CottageReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReservationReview == null)
            {
                return NotFound();
            }

            return View(cottageReservationReview);
        }

        // GET: CottageReservationReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageReservationReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/CottageReservationReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Review,DidntShow,ReceivedPenalty,Id,RowVersion")] CottageReservationReview cottageReservationReview)
        {
            if (ModelState.IsValid)
            {
                cottageReservationReview.Id = Guid.NewGuid();
                cottageReservationReview.ReservationId = id.ToString();
                CottageReservation cottageReservation = _context.CottageReservation.Where(m => m.Id == id).FirstOrDefault<CottageReservation>();
                cottageReservation.IsReviewed = true;
                if (!cottageReservationReview.DidntShow)
                {
                    Guid userId = Guid.Parse(cottageReservation.UserDetailsId);
                    UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                    userDetails.PenaltyCount++;
                    _context.Update(userDetails);
                    await _context.SaveChangesAsync();

                }
                if(cottageReservationReview.ReceivedPenalty)
                {
                    cottageReservationReview.IsReviewedByAdmin = false;
                } else
                {
                    cottageReservationReview.IsReviewedByAdmin = true;
                }
                _context.Add(cottageReservationReview);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/CottagesReservationsHistory", new { area = "Identity" });
            }
            return View(cottageReservationReview);
        }

        // GET: CottageReservationReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationReview = await _context.CottageReservationReview.FindAsync(id);
            if (cottageReservationReview == null)
            {
                return NotFound();
            }
            return View(cottageReservationReview);
        }

        // POST: CottageReservationReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReceivedPenalty,Id,RowVersion")] CottageReservationReview cottageReservationReview)
        {
            if (id != cottageReservationReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var temp = _context.CottageReservationReview.Find(id);
                    temp.ReceivedPenalty = cottageReservationReview.ReceivedPenalty;
                    temp.IsReviewedByAdmin = true;
                    _context.Update(temp);
                    await _context.SaveChangesAsync();
                    if (cottageReservationReview.ReceivedPenalty)
                    {

                        var cottageReservation = _context.CottageReservation.Find(Guid.Parse(temp.ReservationId));
                        Guid userId = Guid.Parse(cottageReservation.UserDetailsId);
                        UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                        userDetails.PenaltyCount++;
                        _context.Update(userDetails);
                        await _context.SaveChangesAsync();

                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageReservationReviewExists(cottageReservationReview.Id))
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
            return View(cottageReservationReview);
        }

        // GET: CottageReservationReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationReview = await _context.CottageReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReservationReview == null)
            {
                return NotFound();
            }

            return View(cottageReservationReview);
        }

        // POST: CottageReservationReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageReservationReview = await _context.CottageReservationReview.FindAsync(id);
            _context.CottageReservationReview.Remove(cottageReservationReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageReservationReviewExists(Guid id)
        {
            return _context.CottageReservationReview.Any(e => e.Id == id);
        }
    }
}
