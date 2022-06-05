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
    public class AdventureReservationFiltersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventureReservationFiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventureReservationFilters
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureReservationFilter.ToListAsync());
        }

        // GET: AdventureReservationFilters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationFilter = await _context.AdventureReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReservationFilter == null)
            {
                return NotFound();
            }

            return View(adventureReservationFilter);
        }

        // GET: AdventureReservationFilters/Create
        public IActionResult Create(Guid? id)
        {
            System.Diagnostics.Debug.Write("prosledjeni id instruktora je: " + id.ToString());

            ViewData["InstructorId"] = id;

            return View();
        }

        
        [HttpGet("/AdventureReservationFilters/AdventureFiltering")]
        public IActionResult AdventureFiltering(Guid? id)
        {
            System.Diagnostics.Debug.Write("prosledjeni id instruktora je: " + id.ToString());

            ViewData["InstructorId"] = id;

            return View();
        }
      
        // POST: AdventureReservationFilters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("startDate,endDate,price,AverageGrade,MaxPersonCount,City,Id,RowVersion")] AdventureReservationFilter adventureReservationFilter)
        {
            if (ModelState.IsValid)
            {
                adventureReservationFilter.Id = Guid.NewGuid();
                _context.Add(adventureReservationFilter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureReservationFilter);
        }

        // GET: AdventureReservationFilters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationFilter = await _context.AdventureReservationFilter.FindAsync(id);
            if (adventureReservationFilter == null)
            {
                return NotFound();
            }
            return View(adventureReservationFilter);
        }

        // POST: AdventureReservationFilters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("startDate,endDate,price,AverageGrade,MaxPersonCount,City,Id,RowVersion")] AdventureReservationFilter adventureReservationFilter)
        {
            if (id != adventureReservationFilter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureReservationFilter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureReservationFilterExists(adventureReservationFilter.Id))
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
            return View(adventureReservationFilter);
        }

        // GET: AdventureReservationFilters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservationFilter = await _context.AdventureReservationFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureReservationFilter == null)
            {
                return NotFound();
            }

            return View(adventureReservationFilter);
        }

        // POST: AdventureReservationFilters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureReservationFilter = await _context.AdventureReservationFilter.FindAsync(id);
            _context.AdventureReservationFilter.Remove(adventureReservationFilter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureReservationFilterExists(Guid id)
        {
            return _context.AdventureReservationFilter.Any(e => e.Id == id);
        }
    }
}
