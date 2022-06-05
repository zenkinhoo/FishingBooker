using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models.DTO;

namespace Hooking.Controllers
{
    public class CottageReservationDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottageReservationDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottageReservationData
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageReservationData.ToListAsync());
        }

        // GET: CottageReservationData/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationData = await _context.CottageReservationData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReservationData == null)
            {
                return NotFound();
            }

            return View(cottageReservationData);
        }

        // GET: CottageReservationData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageReservationData/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,UserDetailsId,StartDate,EndDate,Price,MaxPersonCount,Id,RowVersion")] CottageReservationData cottageReservationData)
        {
            if (ModelState.IsValid)
            {
                cottageReservationData.Id = Guid.NewGuid();
                _context.Add(cottageReservationData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottageReservationData);
        }

        // GET: CottageReservationData/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationData = await _context.CottageReservationData.FindAsync(id);
            if (cottageReservationData == null)
            {
                return NotFound();
            }
            return View(cottageReservationData);
        }

        // POST: CottageReservationData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,UserDetailsId,StartDate,EndDate,Price,MaxPersonCount,Id,RowVersion")] CottageReservationData cottageReservationData)
        {
            if (id != cottageReservationData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageReservationData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageReservationDataExists(cottageReservationData.Id))
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
            return View(cottageReservationData);
        }

        // GET: CottageReservationData/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservationData = await _context.CottageReservationData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReservationData == null)
            {
                return NotFound();
            }

            return View(cottageReservationData);
        }

        // POST: CottageReservationData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageReservationData = await _context.CottageReservationData.FindAsync(id);
            _context.CottageReservationData.Remove(cottageReservationData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageReservationDataExists(Guid id)
        {
            return _context.CottageReservationData.Any(e => e.Id == id);
        }
    }
}
