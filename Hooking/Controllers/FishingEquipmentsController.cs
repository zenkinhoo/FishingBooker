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
    public class FishingEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishingEquipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FishingEquipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.FishingEquipment.ToListAsync());
        }

        // GET: FishingEquipments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingEquipment = await _context.FishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fishingEquipment == null)
            {
                return NotFound();
            }

            return View(fishingEquipment);
        }

        // GET: FishingEquipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FishingEquipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/FishingEquipments/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("LiveBite,FlyFishingGear,Lures,RodsReelsTackle,Id,RowVersion")] FishingEquipment fishingEquipment)
        {
            if (ModelState.IsValid)
            {
                fishingEquipment.Id = Guid.NewGuid();
                _context.Add(fishingEquipment);
                await _context.SaveChangesAsync();
                BoatFishingEquipment boatFishingEquipment = new BoatFishingEquipment();
                boatFishingEquipment.Id = Guid.NewGuid();
                boatFishingEquipment.BoatId = id.ToString();
                boatFishingEquipment.FishingEquipment = fishingEquipment.Id.ToString();
                _context.Add(boatFishingEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Amenities", new { id = id });
            }
            return View(fishingEquipment);
        }

        // GET: FishingEquipments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingEquipment = await _context.FishingEquipment.FindAsync(id);
            if (fishingEquipment == null)
            {
                return NotFound();
            }
            return View(fishingEquipment);
        }

        // POST: FishingEquipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("LiveBite,FlyFishingGear,Lures,RodsReelsTackle,Id,RowVersion")] FishingEquipment fishingEquipment)
        {
            if (id != fishingEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fishingEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FishingEquipmentExists(fishingEquipment.Id))
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
            return View(fishingEquipment);
        }

        // GET: FishingEquipments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishingEquipment = await _context.FishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fishingEquipment == null)
            {
                return NotFound();
            }

            return View(fishingEquipment);
        }

        // POST: FishingEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fishingEquipment = await _context.FishingEquipment.FindAsync(id);
            _context.FishingEquipment.Remove(fishingEquipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FishingEquipmentExists(Guid id)
        {
            return _context.FishingEquipment.Any(e => e.Id == id);
        }
    }
}
