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
    public class CottageOwnerReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottageOwnerReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottageOwnerReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageOwnerReview.ToListAsync());
        }

        // GET: CottageOwnerReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwnerReview = await _context.CottageOwnerReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageOwnerReview == null)
            {
                return NotFound();
            }

            return View(cottageOwnerReview);
        }

        // GET: CottageOwnerReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageOwnerReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageOwnerId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] CottageOwnerReview cottageOwnerReview)
        {
            if (ModelState.IsValid)
            {
                cottageOwnerReview.Id = Guid.NewGuid();
                _context.Add(cottageOwnerReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottageOwnerReview);
        }

        // GET: CottageOwnerReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwnerReview = await _context.CottageOwnerReview.FindAsync(id);
            if (cottageOwnerReview == null)
            {
                return NotFound();
            }
            return View(cottageOwnerReview);
        }

        // POST: CottageOwnerReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageOwnerId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] CottageOwnerReview cottageOwnerReview)
        {
            if (id != cottageOwnerReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageOwnerReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageOwnerReviewExists(cottageOwnerReview.Id))
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
            return View(cottageOwnerReview);
        }

        // GET: CottageOwnerReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwnerReview = await _context.CottageOwnerReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageOwnerReview == null)
            {
                return NotFound();
            }

            return View(cottageOwnerReview);
        }

        // POST: CottageOwnerReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageOwnerReview = await _context.CottageOwnerReview.FindAsync(id);
            _context.CottageOwnerReview.Remove(cottageOwnerReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageOwnerReviewExists(Guid id)
        {
            return _context.CottageOwnerReview.Any(e => e.Id == id);
        }
    }
}
