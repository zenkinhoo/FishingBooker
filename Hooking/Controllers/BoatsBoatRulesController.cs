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
    public class BoatsBoatRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatsBoatRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatsBoatRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatsBoatRules.ToListAsync());
        }

        // GET: BoatsBoatRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatsBoatRules = await _context.BoatsBoatRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatsBoatRules == null)
            {
                return NotFound();
            }

            return View(boatsBoatRules);
        }

        // GET: BoatsBoatRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatsBoatRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,BoatRulesId,Id,RowVersion")] BoatsBoatRules boatsBoatRules)
        {
            if (ModelState.IsValid)
            {
                boatsBoatRules.Id = Guid.NewGuid();
                _context.Add(boatsBoatRules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatsBoatRules);
        }

        // GET: BoatsBoatRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatsBoatRules = await _context.BoatsBoatRules.FindAsync(id);
            if (boatsBoatRules == null)
            {
                return NotFound();
            }
            return View(boatsBoatRules);
        }

        // POST: BoatsBoatRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,BoatRulesId,Id,RowVersion")] BoatsBoatRules boatsBoatRules)
        {
            if (id != boatsBoatRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatsBoatRules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatsBoatRulesExists(boatsBoatRules.Id))
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
            return View(boatsBoatRules);
        }

        // GET: BoatsBoatRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatsBoatRules = await _context.BoatsBoatRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatsBoatRules == null)
            {
                return NotFound();
            }

            return View(boatsBoatRules);
        }

        // POST: BoatsBoatRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatsBoatRules = await _context.BoatsBoatRules.FindAsync(id);
            _context.BoatsBoatRules.Remove(boatsBoatRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatsBoatRulesExists(Guid id)
        {
            return _context.BoatsBoatRules.Any(e => e.Id == id);
        }
    }
}
