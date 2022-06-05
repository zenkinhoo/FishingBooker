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
    public class AmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Amenities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Amenities.ToListAsync());
        }

        // GET: Amenities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenities = await _context.Amenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (amenities == null)
            {
                return NotFound();
            }

            return View(amenities);
        }

        // GET: Amenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amenities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Amenities/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Gps,Radar,VhrRadio,Sonar,FishFinder,WiFi,Id,RowVersion")] Amenities amenities)
        {
            if (ModelState.IsValid)
            {
                amenities.Id = Guid.NewGuid();
                _context.Add(amenities);
                await _context.SaveChangesAsync();
                BoatAmenities boatAmenities = new BoatAmenities();
                boatAmenities.BoatId = id.ToString();
                boatAmenities.AmanitiesId = amenities.Id.ToString();
                boatAmenities.Id = Guid.NewGuid();
                _context.BoatAmenities.Add(boatAmenities);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/MyBoats", new { area = "Identity" });
            }
            return View(amenities);
        }

        // GET: Amenities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenities = await _context.Amenities.FindAsync(id);
            if (amenities == null)
            {
                return NotFound();
            }
            return View(amenities);
        }

        // POST: Amenities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Gps,Radar,VhrRadio,Sonar,FishFinder,WiFi,Id,RowVersion")] Amenities amenities)
        {
            if (id != amenities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(amenities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmenitiesExists(amenities.Id))
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
            return View(amenities);
        }

        // GET: Amenities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenities = await _context.Amenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (amenities == null)
            {
                return NotFound();
            }

            return View(amenities);
        }

        // POST: Amenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var amenities = await _context.Amenities.FindAsync(id);
            _context.Amenities.Remove(amenities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AmenitiesExists(Guid id)
        {
            return _context.Amenities.Any(e => e.Id == id);
        }
    }
}
