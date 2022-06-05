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
    public class AdventureFishingTechniquesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventureFishingTechniquesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventureFishingTechniques
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureFishingTechniques.ToListAsync());
        }

        // GET: AdventureFishingTechniques/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingTechniques = await _context.AdventureFishingTechniques
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFishingTechniques == null)
            {
                return NotFound();
            }

            return View(adventureFishingTechniques);
        }

        // GET: AdventureFishingTechniques/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureFishingTechniques/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdventureId,FishingTechniquesId,Id,RowVersion")] AdventureFishingTechniques adventureFishingTechniques)
        {
            if (ModelState.IsValid)
            {
                adventureFishingTechniques.Id = Guid.NewGuid();
                _context.Add(adventureFishingTechniques);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureFishingTechniques);
        }

        // GET: AdventureFishingTechniques/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingTechniques = await _context.AdventureFishingTechniques.FindAsync(id);
            if (adventureFishingTechniques == null)
            {
                return NotFound();
            }
            return View(adventureFishingTechniques);
        }

        // POST: AdventureFishingTechniques/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AdventureId,FishingTechniquesId,Id,RowVersion")] AdventureFishingTechniques adventureFishingTechniques)
        {
            if (id != adventureFishingTechniques.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureFishingTechniques);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureFishingTechniquesExists(adventureFishingTechniques.Id))
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
            return View(adventureFishingTechniques);
        }

        // GET: AdventureFishingTechniques/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingTechniques = await _context.AdventureFishingTechniques
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFishingTechniques == null)
            {
                return NotFound();
            }

            return View(adventureFishingTechniques);
        }

        // POST: AdventureFishingTechniques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureFishingTechniques = await _context.AdventureFishingTechniques.FindAsync(id);
            _context.AdventureFishingTechniques.Remove(adventureFishingTechniques);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureFishingTechniquesExists(Guid id)
        {
            return _context.AdventureFishingTechniques.Any(e => e.Id == id);
        }
    }
}
