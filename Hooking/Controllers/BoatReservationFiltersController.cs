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
    public class BoatReservationFiltersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatReservationFiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatReservationFilters
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatReservationFilter.ToListAsync());
        }

        // GET: BoatReservationFilters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationFilter = await _context.BoatReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReservationFilter == null)
            {
                return NotFound();
            }

            return View(boatReservationFilter);
        }

        // GET: BoatReservationFilters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatReservationFilters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("startDate,endDate,price,AverageGrade,MaxPersonCount,City,Id,RowVersion")] BoatReservationFilter boatReservationFilter)
        {
            if (ModelState.IsValid)
            {
                boatReservationFilter.Id = Guid.NewGuid();
                _context.Add(boatReservationFilter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatReservationFilter);
        }

        // GET: BoatReservationFilters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationFilter = await _context.BoatReservationFilter.FindAsync(id);
            if (boatReservationFilter == null)
            {
                return NotFound();
            }
            return View(boatReservationFilter);
        }

        // POST: BoatReservationFilters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("startDate,endDate,price,AverageGrade,MaxPersonCount,City,Id,RowVersion")] BoatReservationFilter boatReservationFilter)
        {
            if (id != boatReservationFilter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatReservationFilter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatReservationFilterExists(boatReservationFilter.Id))
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
            return View(boatReservationFilter);
        }

        // GET: BoatReservationFilters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservationFilter = await _context.BoatReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReservationFilter == null)
            {
                return NotFound();
            }

            return View(boatReservationFilter);
        }

        // POST: BoatReservationFilters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatReservationFilter = await _context.BoatReservationFilter.FindAsync(id);
            _context.BoatReservationFilter.Remove(boatReservationFilter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatReservationFilterExists(Guid id)
        {
            return _context.BoatReservationFilter.Any(e => e.Id == id);
        }
    }
}
