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
    public class BoatRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatRules.ToListAsync());
        }

        // GET: BoatRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatRules = await _context.BoatRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatRules == null)
            {
                return NotFound();
            }

            return View(boatRules);
        }

        // GET: BoatRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("BoatRules/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("ChildFriendly,YouKeepCatch,CatchAndReleaseAllowed,CabinSmoking,Id,RowVersion")] BoatRules boatRules)
        {
            if (ModelState.IsValid)
            {
                boatRules.Id = Guid.NewGuid();
                _context.Add(boatRules);
                await _context.SaveChangesAsync();
                BoatsBoatRules boatsBoatRules = new BoatsBoatRules();
                boatsBoatRules.Id = Guid.NewGuid();
                boatsBoatRules.BoatId = id.ToString();
                boatsBoatRules.BoatRulesId = boatRules.Id.ToString();
                _context.Add(boatsBoatRules);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreateForBoat", "CancelationPolicies", new { id = boatsBoatRules.BoatId });
            }
            return View(boatRules);
        }

        // GET: BoatRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatRules = await _context.BoatRules.FindAsync(id);
            if (boatRules == null)
            {
                return NotFound();
            }
            return View(boatRules);
        }

        // POST: BoatRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ChildFriendly,YouKeepCatch,CatchAndReleaseAllowed,CabinSmoking,Id,RowVersion")] BoatRules boatRules)
        {
            if (id != boatRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatRules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatRulesExists(boatRules.Id))
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
            return View(boatRules);
        }

        // GET: BoatRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatRules = await _context.BoatRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatRules == null)
            {
                return NotFound();
            }

            return View(boatRules);
        }

        // POST: BoatRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatRules = await _context.BoatRules.FindAsync(id);
            _context.BoatRules.Remove(boatRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatRulesExists(Guid id)
        {
            return _context.BoatRules.Any(e => e.Id == id);
        }
    }
}
