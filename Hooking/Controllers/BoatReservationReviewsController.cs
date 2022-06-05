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
    public class BoatReservationReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatReservationReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatReservationReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatReservationReview.ToListAsync());
        }

        // GET: BoatReservationReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationReview = await _context.BoatReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReservationReview == null)
            {
                return NotFound();
            }

            return View(boatReservationReview);
        }

        // GET: BoatReservationReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatReservationReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/BoatReservationReviews/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Review,DidntShow,ReceivedPenalty,Id,RowVersion")] BoatReservationReview boatReservationReview)
        {
            if (ModelState.IsValid)
            {
                boatReservationReview.Id = Guid.NewGuid();
                boatReservationReview.BoatReservationId = id.ToString();
                boatReservationReview.IsReviewedByAdmin = false;
                Guid reservationId = Guid.Parse(boatReservationReview.BoatReservationId);
                BoatReservation boatReservation = _context.BoatReservation.Where(m => m.Id == reservationId).FirstOrDefault();
                if (!boatReservationReview.DidntShow)
                {
                    Guid userId = Guid.Parse(boatReservation.UserDetailsId);
                    UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                    userDetails.PenaltyCount++;
                    _context.Update(userDetails);
                    await _context.SaveChangesAsync();

                }
                if (boatReservationReview.ReceivedPenalty)
                {
                    boatReservationReview.IsReviewedByAdmin = false;
                }
                else
                {
                    boatReservationReview.IsReviewedByAdmin = true;
                }
                _context.Add(boatReservationReview);
                await _context.SaveChangesAsync();
                boatReservation.IsReviewed = true;
                _context.BoatReservation.Update(boatReservation);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/BoatReservationsHistory", new { area = "Identity" });
            }
            return View(boatReservationReview);
        }

        // GET: BoatReservationReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationReview = await _context.BoatReservationReview.FindAsync(id);
            if (boatReservationReview == null)
            {
                return NotFound();
            }
            return View(boatReservationReview);
        }

        // POST: BoatReservationReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReceivedPenalty,Id,RowVersion")] BoatReservationReview boatReservationReview)
        {
            if (id != boatReservationReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var temp = _context.BoatReservationReview.Find(id);
                    temp.IsReviewedByAdmin = true;
                    temp.ReceivedPenalty = boatReservationReview.ReceivedPenalty;
                    _context.Update(temp);
                    await _context.SaveChangesAsync();
                    if (boatReservationReview.ReceivedPenalty)
                    {
                        var boatReservation = _context.BoatReservation.Find(Guid.Parse(temp.BoatReservationId));
                        Guid userId = Guid.Parse(boatReservation.UserDetailsId);
                        UserDetails userDetails = _context.UserDetails.Where(m => m.Id == userId).FirstOrDefault<UserDetails>();
                        userDetails.PenaltyCount++;
                        _context.Update(userDetails);
                        await _context.SaveChangesAsync();

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatReservationReviewExists(boatReservationReview.Id))
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
            return View(boatReservationReview);
        }

        // GET: BoatReservationReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationReview = await _context.BoatReservationReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReservationReview == null)
            {
                return NotFound();
            }

            return View(boatReservationReview);
        }

        // POST: BoatReservationReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatReservationReview = await _context.BoatReservationReview.FindAsync(id);
            _context.BoatReservationReview.Remove(boatReservationReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatReservationReviewExists(Guid id)
        {
            return _context.BoatReservationReview.Any(e => e.Id == id);
        }
    }
}
