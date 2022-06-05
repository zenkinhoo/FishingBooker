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
    public class BoatSpecialOfferReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoatSpecialOfferReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BoatSpecialOfferReservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatSpecialOfferReservation.ToListAsync());
        }

        // GET: BoatSpecialOfferReservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOfferReservation = await _context.BoatSpecialOfferReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatSpecialOfferReservation == null)
            {
                return NotFound();
            }

            return View(boatSpecialOfferReservation);
        }

        // GET: BoatSpecialOfferReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatSpecialOfferReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatSpecialOfferId,UserDetailsId,Id,RowVersion")] BoatSpecialOfferReservation boatSpecialOfferReservation)
        {
            if (ModelState.IsValid)
            {
                boatSpecialOfferReservation.Id = Guid.NewGuid();
                _context.Add(boatSpecialOfferReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatSpecialOfferReservation);
        }

        // GET: BoatSpecialOfferReservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOfferReservation = await _context.BoatSpecialOfferReservation.FindAsync(id);
            if (boatSpecialOfferReservation == null)
            {
                return NotFound();
            }
            return View(boatSpecialOfferReservation);
        }

        // POST: BoatSpecialOfferReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatSpecialOfferId,UserDetailsId,Id,RowVersion")] BoatSpecialOfferReservation boatSpecialOfferReservation)
        {
            if (id != boatSpecialOfferReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatSpecialOfferReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatSpecialOfferReservationExists(boatSpecialOfferReservation.Id))
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
            return View(boatSpecialOfferReservation);
        }

        // GET: BoatSpecialOfferReservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOfferReservation = await _context.BoatSpecialOfferReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatSpecialOfferReservation == null)
            {
                return NotFound();
            }

            return View(boatSpecialOfferReservation);
        }

        // POST: BoatSpecialOfferReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatSpecialOfferReservation = await _context.BoatSpecialOfferReservation.FindAsync(id);
            _context.BoatSpecialOfferReservation.Remove(boatSpecialOfferReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatSpecialOfferReservationExists(Guid id)
        {
            return _context.BoatSpecialOfferReservation.Any(e => e.Id == id);
        }
    }
}
