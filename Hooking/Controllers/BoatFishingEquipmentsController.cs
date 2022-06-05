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
    public class BoatFishingEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatFishingEquipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatFishingEquipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatFishingEquipment.ToListAsync());
        }

        // GET: BoatFishingEquipments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFishingEquipment = await _context.BoatFishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatFishingEquipment == null)
            {
                return NotFound();
            }

            return View(boatFishingEquipment);
        }

        // GET: BoatFishingEquipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatFishingEquipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,FishingEquipment,Id,RowVersion")] BoatFishingEquipment boatFishingEquipment)
        {
            if (ModelState.IsValid)
            {
                boatFishingEquipment.Id = Guid.NewGuid();
                _context.Add(boatFishingEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatFishingEquipment);
        }

        // GET: BoatFishingEquipments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFishingEquipment = await _context.BoatFishingEquipment.FindAsync(id);
            if (boatFishingEquipment == null)
            {
                return NotFound();
            }
            return View(boatFishingEquipment);
        }

        // POST: BoatFishingEquipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,FishingEquipment,Id,RowVersion")] BoatFishingEquipment boatFishingEquipment)
        {
            if (id != boatFishingEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatFishingEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatFishingEquipmentExists(boatFishingEquipment.Id))
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
            return View(boatFishingEquipment);
        }

        // GET: BoatFishingEquipments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFishingEquipment = await _context.BoatFishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatFishingEquipment == null)
            {
                return NotFound();
            }

            return View(boatFishingEquipment);
        }

        // POST: BoatFishingEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatFishingEquipment = await _context.BoatFishingEquipment.FindAsync(id);
            _context.BoatFishingEquipment.Remove(boatFishingEquipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatFishingEquipmentExists(Guid id)
        {
            return _context.BoatFishingEquipment.Any(e => e.Id == id);
        }
    }
}
