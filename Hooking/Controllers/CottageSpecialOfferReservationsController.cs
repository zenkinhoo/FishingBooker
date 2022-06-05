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
    public class CottageSpecialOfferReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottageSpecialOfferReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottageSpecialOfferReservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageSpecialOfferReservation.ToListAsync());
        }

        // GET: CottageSpecialOfferReservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOfferReservation = await _context.CottageSpecialOfferReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageSpecialOfferReservation == null)
            {
                return NotFound();
            }

            return View(cottageSpecialOfferReservation);
        }

        // GET: CottageSpecialOfferReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageSpecialOfferReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageSpecialOfferId,UserDetailsId,Id,RowVersion")] CottageSpecialOfferReservation cottageSpecialOfferReservation)
        {
            if (ModelState.IsValid)
            {
                cottageSpecialOfferReservation.Id = Guid.NewGuid();
                _context.Add(cottageSpecialOfferReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottageSpecialOfferReservation);
        }

        // GET: CottageSpecialOfferReservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOfferReservation = await _context.CottageSpecialOfferReservation.FindAsync(id);
            if (cottageSpecialOfferReservation == null)
            {
                return NotFound();
            }
            return View(cottageSpecialOfferReservation);
        }

        // POST: CottageSpecialOfferReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageSpecialOfferId,UserDetailsId,Id,RowVersion")] CottageSpecialOfferReservation cottageSpecialOfferReservation)
        {
            if (id != cottageSpecialOfferReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageSpecialOfferReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageSpecialOfferReservationExists(cottageSpecialOfferReservation.Id))
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
            return View(cottageSpecialOfferReservation);
        }

        // GET: CottageSpecialOfferReservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOfferReservation = await _context.CottageSpecialOfferReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageSpecialOfferReservation == null)
            {
                return NotFound();
            }

            return View(cottageSpecialOfferReservation);
        }

        // POST: CottageSpecialOfferReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageSpecialOfferReservation = await _context.CottageSpecialOfferReservation.FindAsync(id);
            _context.CottageSpecialOfferReservation.Remove(cottageSpecialOfferReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageSpecialOfferReservationExists(Guid id)
        {
            return _context.CottageSpecialOfferReservation.Any(e => e.Id == id);
        }
    }
}
