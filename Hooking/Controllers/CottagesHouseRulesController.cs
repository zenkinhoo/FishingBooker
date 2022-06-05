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
    public class CottagesHouseRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottagesHouseRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottagesHouseRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottagesHouseRules.ToListAsync());
        }

        // GET: CottagesHouseRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesHouseRules = await _context.CottagesHouseRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesHouseRules == null)
            {
                return NotFound();
            }

            return View(cottagesHouseRules);
        }

        // GET: CottagesHouseRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottagesHouseRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,HouseRulesId,Id,RowVersion")] CottagesHouseRules cottagesHouseRules)
        {
            if (ModelState.IsValid)
            {
                cottagesHouseRules.Id = Guid.NewGuid();
                _context.Add(cottagesHouseRules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottagesHouseRules);
        }

        // GET: CottagesHouseRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesHouseRules = await _context.CottagesHouseRules.FindAsync(id);
            if (cottagesHouseRules == null)
            {
                return NotFound();
            }
            return View(cottagesHouseRules);
        }

        // POST: CottagesHouseRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,HouseRulesId,Id,RowVersion")] CottagesHouseRules cottagesHouseRules)
        {
            if (id != cottagesHouseRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottagesHouseRules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottagesHouseRulesExists(cottagesHouseRules.Id))
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
            return View(cottagesHouseRules);
        }

        // GET: CottagesHouseRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesHouseRules = await _context.CottagesHouseRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesHouseRules == null)
            {
                return NotFound();
            }

            return View(cottagesHouseRules);
        }

        // POST: CottagesHouseRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottagesHouseRules = await _context.CottagesHouseRules.FindAsync(id);
            _context.CottagesHouseRules.Remove(cottagesHouseRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottagesHouseRulesExists(Guid id)
        {
            return _context.CottagesHouseRules.Any(e => e.Id == id);
        }
    }
}
