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
    public class AdventureFishingEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdventureFishingEquipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdventureFishingEquipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureFishingEquipment.ToListAsync());
        }

        // GET: AdventureFishingEquipments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingEquipment = await _context.AdventureFishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFishingEquipment == null)
            {
                return NotFound();
            }

            return View(adventureFishingEquipment);
        }

        // GET: AdventureFishingEquipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureFishingEquipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdventureId,FishingEquipmentId,Id,RowVersion")] AdventureFishingEquipment adventureFishingEquipment)
        {
            if (ModelState.IsValid)
            {
                adventureFishingEquipment.Id = Guid.NewGuid();
                _context.Add(adventureFishingEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureFishingEquipment);
        }

        // GET: AdventureFishingEquipments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingEquipment = await _context.AdventureFishingEquipment.FindAsync(id);
            if (adventureFishingEquipment == null)
            {
                return NotFound();
            }
            return View(adventureFishingEquipment);
        }

        // POST: AdventureFishingEquipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AdventureId,FishingEquipmentId,Id,RowVersion")] AdventureFishingEquipment adventureFishingEquipment)
        {
            if (id != adventureFishingEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureFishingEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureFishingEquipmentExists(adventureFishingEquipment.Id))
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
            return View(adventureFishingEquipment);
        }

        // GET: AdventureFishingEquipments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFishingEquipment = await _context.AdventureFishingEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFishingEquipment == null)
            {
                return NotFound();
            }

            return View(adventureFishingEquipment);
        }

        // POST: AdventureFishingEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureFishingEquipment = await _context.AdventureFishingEquipment.FindAsync(id);
            _context.AdventureFishingEquipment.Remove(adventureFishingEquipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureFishingEquipmentExists(Guid id)
        {
            return _context.AdventureFishingEquipment.Any(e => e.Id == id);
        }
    }
}
