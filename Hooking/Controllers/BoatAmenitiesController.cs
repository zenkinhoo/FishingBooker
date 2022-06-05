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
    public class BoatAmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatAmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatAmenities
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatAmenities.ToListAsync());
        }

        // GET: BoatAmenities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAmenities = await _context.BoatAmenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatAmenities == null)
            {
                return NotFound();
            }

            return View(boatAmenities);
        }

        // GET: BoatAmenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatAmenities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,AmanitiesId,Id,RowVersion")] BoatAmenities boatAmenities)
        {
            if (ModelState.IsValid)
            {
                boatAmenities.Id = Guid.NewGuid();
                _context.Add(boatAmenities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatAmenities);
        }

        // GET: BoatAmenities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAmenities = await _context.BoatAmenities.FindAsync(id);
            if (boatAmenities == null)
            {
                return NotFound();
            }
            return View(boatAmenities);
        }

        // POST: BoatAmenities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,AmanitiesId,Id,RowVersion")] BoatAmenities boatAmenities)
        {
            if (id != boatAmenities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatAmenities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatAmenitiesExists(boatAmenities.Id))
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
            return View(boatAmenities);
        }

        // GET: BoatAmenities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatAmenities = await _context.BoatAmenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatAmenities == null)
            {
                return NotFound();
            }

            return View(boatAmenities);
        }

        // POST: BoatAmenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatAmenities = await _context.BoatAmenities.FindAsync(id);
            _context.BoatAmenities.Remove(boatAmenities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatAmenitiesExists(Guid id)
        {
            return _context.BoatAmenities.Any(e => e.Id == id);
        }
    }
}
