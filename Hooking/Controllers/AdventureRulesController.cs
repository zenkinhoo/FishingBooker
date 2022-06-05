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
    public class AdventureRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventureRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventureRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureRules.ToListAsync());
        }

        // GET: AdventureRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureRules = await _context.AdventureRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureRules == null)
            {
                return NotFound();
            }

            return View(adventureRules);
        }

        // GET: AdventureRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChildFriendly,YouKeepCatch,CatchAndReleaseAllowed,CabinSmoking,Id,RowVersion")] AdventureRules adventureRules)
        {
            if (ModelState.IsValid)
            {
                adventureRules.Id = Guid.NewGuid();
                _context.Add(adventureRules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureRules);
        }

        // GET: AdventureRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureRules = await _context.AdventureRules.FindAsync(id);
            if (adventureRules == null)
            {
                return NotFound();
            }
            return View(adventureRules);
        }

        // POST: AdventureRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ChildFriendly,YouKeepCatch,CatchAndReleaseAllowed,CabinSmoking,Id,RowVersion")] AdventureRules adventureRules)
        {
            if (id != adventureRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureRules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureRulesExists(adventureRules.Id))
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
            return View(adventureRules);
        }

        // GET: AdventureRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureRules = await _context.AdventureRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureRules == null)
            {
                return NotFound();
            }

            return View(adventureRules);
        }

        // POST: AdventureRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureRules = await _context.AdventureRules.FindAsync(id);
            _context.AdventureRules.Remove(adventureRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureRulesExists(Guid id)
        {
            return _context.AdventureRules.Any(e => e.Id == id);
        }
    }
}
