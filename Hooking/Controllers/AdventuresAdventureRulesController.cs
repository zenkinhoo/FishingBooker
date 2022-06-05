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
    public class AdventuresAdventureRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventuresAdventureRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventuresAdventureRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventuresAdventureRules.ToListAsync());
        }

        // GET: AdventuresAdventureRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventuresAdventureRules = await _context.AdventuresAdventureRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventuresAdventureRules == null)
            {
                return NotFound();
            }

            return View(adventuresAdventureRules);
        }

        // GET: AdventuresAdventureRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventuresAdventureRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdventureId,AdventureRulesId,Id,RowVersion")] AdventuresAdventureRules adventuresAdventureRules)
        {
            if (ModelState.IsValid)
            {
                adventuresAdventureRules.Id = Guid.NewGuid();
                _context.Add(adventuresAdventureRules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventuresAdventureRules);
        }

        // GET: AdventuresAdventureRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventuresAdventureRules = await _context.AdventuresAdventureRules.FindAsync(id);
            if (adventuresAdventureRules == null)
            {
                return NotFound();
            }
            return View(adventuresAdventureRules);
        }

        // POST: AdventuresAdventureRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AdventureId,AdventureRulesId,Id,RowVersion")] AdventuresAdventureRules adventuresAdventureRules)
        {
            if (id != adventuresAdventureRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventuresAdventureRules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventuresAdventureRulesExists(adventuresAdventureRules.Id))
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
            return View(adventuresAdventureRules);
        }

        // GET: AdventuresAdventureRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventuresAdventureRules = await _context.AdventuresAdventureRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventuresAdventureRules == null)
            {
                return NotFound();
            }

            return View(adventuresAdventureRules);
        }

        // POST: AdventuresAdventureRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventuresAdventureRules = await _context.AdventuresAdventureRules.FindAsync(id);
            _context.AdventuresAdventureRules.Remove(adventuresAdventureRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventuresAdventureRulesExists(Guid id)
        {
            return _context.AdventuresAdventureRules.Any(e => e.Id == id);
        }
    }
}
