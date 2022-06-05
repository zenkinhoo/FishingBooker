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
    public class FishingTechniquesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishingTechniquesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FishingTechniques
        public async Task<IActionResult> Index()
        {
            return View(await _context.FishingTechniques.ToListAsync());
        }

        // GET: FishingTechniques/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingTechniques = await _context.FishingTechniques
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fishingTechniques == null)
            {
                return NotFound();
            }

            return View(fishingTechniques);
        }

        // GET: FishingTechniques/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FishingTechniques/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorHasBoat,Inshore,Offshore,Id,RowVersion")] FishingTechniques fishingTechniques)
        {
            if (ModelState.IsValid)
            {
                fishingTechniques.Id = Guid.NewGuid();
                _context.Add(fishingTechniques);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fishingTechniques);
        }

        // GET: FishingTechniques/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingTechniques = await _context.FishingTechniques.FindAsync(id);
            if (fishingTechniques == null)
            {
                return NotFound();
            }
            return View(fishingTechniques);
        }

        // POST: FishingTechniques/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InstructorHasBoat,Inshore,Offshore,Id,RowVersion")] FishingTechniques fishingTechniques)
        {
            if (id != fishingTechniques.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fishingTechniques);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FishingTechniquesExists(fishingTechniques.Id))
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
            return View(fishingTechniques);
        }

        // GET: FishingTechniques/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingTechniques = await _context.FishingTechniques
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fishingTechniques == null)
            {
                return NotFound();
            }

            return View(fishingTechniques);
        }

        // POST: FishingTechniques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fishingTechniques = await _context.FishingTechniques.FindAsync(id);
            _context.FishingTechniques.Remove(fishingTechniques);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FishingTechniquesExists(Guid id)
        {
            return _context.FishingTechniques.Any(e => e.Id == id);
        }
    }
}
