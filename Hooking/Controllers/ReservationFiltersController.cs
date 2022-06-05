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
    public class ReservationFiltersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationFiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReservationFilters
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservationFilter.ToListAsync());
        }

        // GET: ReservationFilters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationFilter = await _context.ReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationFilter == null)
            {
                return NotFound();
            }

            return View(reservationFilter);
        }

        // GET: ReservationFilters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReservationFilters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("startDate,endDate,price,City,AverageGrade,MaxPersonCount,Id,RowVersion")] ReservationFilter reservationFilter)
        {
            if (ModelState.IsValid)
            {
                reservationFilter.Id = Guid.NewGuid();
                _context.Add(reservationFilter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservationFilter);
        }

        // GET: ReservationFilters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationFilter = await _context.ReservationFilter.FindAsync(id);
            if (reservationFilter == null)
            {
                return NotFound();
            }
            return View(reservationFilter);
        }

        // POST: ReservationFilters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("startDate,endDate,price,City,AverageGrade,MaxPersonCount,Id,RowVersion")] ReservationFilter reservationFilter)
        {
            if (id != reservationFilter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservationFilter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationFilterExists(reservationFilter.Id))
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
            return View(reservationFilter);
        }

        // GET: ReservationFilters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationFilter = await _context.ReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservationFilter == null)
            {
                return NotFound();
            }

            return View(reservationFilter);
        }

        // POST: ReservationFilters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var reservationFilter = await _context.ReservationFilter.FindAsync(id);
            _context.ReservationFilter.Remove(reservationFilter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationFilterExists(Guid id)
        {
            return _context.ReservationFilter.Any(e => e.Id == id);
        }
    }
}
