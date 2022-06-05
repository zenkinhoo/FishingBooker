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
    public class BoatOwnerReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatOwnerReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatOwnerReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatOwnerReview.ToListAsync());
        }

        // GET: BoatOwnerReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwnerReview = await _context.BoatOwnerReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatOwnerReview == null)
            {
                return NotFound();
            }

            return View(boatOwnerReview);
        }

        // GET: BoatOwnerReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatOwnerReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatOwnerId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] BoatOwnerReview boatOwnerReview)
        {
            if (ModelState.IsValid)
            {
                boatOwnerReview.Id = Guid.NewGuid();
                _context.Add(boatOwnerReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatOwnerReview);
        }

        // GET: BoatOwnerReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwnerReview = await _context.BoatOwnerReview.FindAsync(id);
            if (boatOwnerReview == null)
            {
                return NotFound();
            }
            return View(boatOwnerReview);
        }

        // POST: BoatOwnerReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatOwnerId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] BoatOwnerReview boatOwnerReview)
        {
            if (id != boatOwnerReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatOwnerReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatOwnerReviewExists(boatOwnerReview.Id))
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
            return View(boatOwnerReview);
        }

        // GET: BoatOwnerReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwnerReview = await _context.BoatOwnerReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatOwnerReview == null)
            {
                return NotFound();
            }

            return View(boatOwnerReview);
        }

        // POST: BoatOwnerReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatOwnerReview = await _context.BoatOwnerReview.FindAsync(id);
            _context.BoatOwnerReview.Remove(boatOwnerReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatOwnerReviewExists(Guid id)
        {
            return _context.BoatOwnerReview.Any(e => e.Id == id);
        }
    }
}
