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
    public class InstructorReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InstructorReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.InstructorReview.ToListAsync());
        }

        // GET: InstructorReviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorReview = await _context.InstructorReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorReview == null)
            {
                return NotFound();
            }

            return View(instructorReview);
        }

        // GET: InstructorReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InstructorReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] InstructorReview instructorReview)
        {
            if (ModelState.IsValid)
            {
                instructorReview.Id = Guid.NewGuid();
                _context.Add(instructorReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructorReview);
        }

        // GET: InstructorReviews/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorReview = await _context.InstructorReview.FindAsync(id);
            if (instructorReview == null)
            {
                return NotFound();
            }
            return View(instructorReview);
        }

        // POST: InstructorReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InstructorId,UserDetailsId,Review,Grade,IsApproved,Id,RowVersion")] InstructorReview instructorReview)
        {
            if (id != instructorReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructorReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorReviewExists(instructorReview.Id))
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
            return View(instructorReview);
        }

        // GET: InstructorReviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorReview = await _context.InstructorReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorReview == null)
            {
                return NotFound();
            }

            return View(instructorReview);
        }

        // POST: InstructorReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var instructorReview = await _context.InstructorReview.FindAsync(id);
            _context.InstructorReview.Remove(instructorReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorReviewExists(Guid id)
        {
            return _context.InstructorReview.Any(e => e.Id == id);
        }
    }
}
