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
    public class BoatOwnersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatOwnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatOwners
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatOwner.ToListAsync());
        }

        // GET: BoatOwners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwner = await _context.BoatOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatOwner == null)
            {
                return NotFound();
            }

            return View(boatOwner);
        }

        // GET: BoatOwners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatOwners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserDetailsId,AverageGrade,GradeCount,IsCaptain,IsFirstOfficer,Id,RowVersion")] BoatOwner boatOwner)
        {
            if (ModelState.IsValid)
            {
                boatOwner.Id = Guid.NewGuid();
                _context.Add(boatOwner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatOwner);
        }

        // GET: BoatOwners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwner = await _context.BoatOwner.FindAsync(id);
            if (boatOwner == null)
            {
                return NotFound();
            }
            return View(boatOwner);
        }

        // POST: BoatOwners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,AverageGrade,GradeCount,IsCaptain,IsFirstOfficer,Id,RowVersion")] BoatOwner boatOwner)
        {
            if (id != boatOwner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatOwnerExists(boatOwner.Id))
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
            return View(boatOwner);
        }

        // GET: BoatOwners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatOwner = await _context.BoatOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatOwner == null)
            {
                return NotFound();
            }

            return View(boatOwner);
        }

        // POST: BoatOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatOwner = await _context.BoatOwner.FindAsync(id);
            _context.BoatOwner.Remove(boatOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatOwnerExists(Guid id)
        {
            return _context.BoatOwner.Any(e => e.Id == id);
        }
    }
}
