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
    public class FacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Facilities.ToListAsync());
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilities = await _context.Facilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facilities == null)
            {
                return NotFound();
            }

            return View(facilities);
        }

        // GET: Facilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Facilities/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("Parking,Wifi,Heating,BarbecueFacilities,OnlineCheckin,Jacuzzi,SeaView,MountainView,Kitchen,WashingMachine,AirportShuttle,IndoorPool,OutdoorPool,StockedBar,Garden,Id,RowVersion")] Facilities facilities)
        {
            if (ModelState.IsValid)
            {
                facilities.Id = Guid.NewGuid();
                _context.Add(facilities);
                await _context.SaveChangesAsync();
                CottagesFacilities cottagesFacilities = new CottagesFacilities();
                cottagesFacilities.Id = Guid.NewGuid();
                cottagesFacilities.CottageId = id.ToString();
                cottagesFacilities.FacilitiesId = facilities.Id.ToString();
                _context.Add(cottagesFacilities);
                await _context.SaveChangesAsync();
                return RedirectToAction("AddRooms" , "CottageRooms" , new { id = Guid.Parse(cottagesFacilities.CottageId)});
            }
            return View(facilities);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilities = await _context.Facilities.FindAsync(id);
            if (facilities == null)
            {
                return NotFound();
            }
            return View(facilities);
        }

        // POST: Facilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Facilities/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Parking,Wifi,Heating,BarbecueFacilities,OnlineCheckin,Jacuzzi,SeaView,MountainView,Kitchen,WashingMachine,AirportShuttle,IndoorPool,OutdoorPool,StockedBar,Garden,Id,RowVersion")] Facilities facilities)
        {
            if (id != facilities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var facilitiesTmp = await _context.Facilities.FindAsync(id);
                    facilitiesTmp.Parking = facilities.Parking;
                    facilitiesTmp.Wifi = facilities.Wifi;
                    facilitiesTmp.Heating = facilities.Heating;
                    facilitiesTmp.BarbecueFacilities = facilities.BarbecueFacilities;
                    facilitiesTmp.OnlineCheckin = facilities.OnlineCheckin;
                    facilitiesTmp.Jacuzzi = facilities.Jacuzzi;
                    facilitiesTmp.SeaView = facilities.SeaView;
                    facilitiesTmp.MountainView = facilities.MountainView;
                    facilitiesTmp.Kitchen = facilities.Kitchen;
                    facilitiesTmp.WashingMachine = facilities.WashingMachine;
                    facilitiesTmp.AirportShuttle = facilities.AirportShuttle;
                    facilitiesTmp.Garden = facilities.Garden;
                    facilitiesTmp.IndoorPool = facilities.IndoorPool;
                    facilities.OutdoorPool = facilities.OutdoorPool;
                    facilitiesTmp.StockedBar = facilities.StockedBar;
                    _context.Update(facilitiesTmp);
                    await _context.SaveChangesAsync();
                    var facilitiesId = facilities.Id.ToString();
                    CottagesFacilities cottagesFacilities = _context.CottagesFacilities.Where(m => m.FacilitiesId == facilitiesId).FirstOrDefault<CottagesFacilities>();
                    Guid cottageId = Guid.Parse(cottagesFacilities.CottageId);
                    var cottage = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault<Cottage>();
                    return RedirectToAction("MyCottage", "Cottages", new { id = cottage.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilitiesExists(facilities.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
            return View(facilities);
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilities = await _context.Facilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facilities == null)
            {
                return NotFound();
            }

            return View(facilities);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var facilities = await _context.Facilities.FindAsync(id);
            _context.Facilities.Remove(facilities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilitiesExists(Guid id)
        {
            return _context.Facilities.Any(e => e.Id == id);
        }
    }
}
