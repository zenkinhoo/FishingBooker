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
    public class CottagesFacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottagesFacilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottagesFacilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottagesFacilities.ToListAsync());
        }

        // GET: CottagesFacilities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesFacilities = await _context.CottagesFacilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesFacilities == null)
            {
                return NotFound();
            }

            return View(cottagesFacilities);
        }

        // GET: CottagesFacilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottagesFacilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,FacilitiesId,Id,RowVersion")] CottagesFacilities cottagesFacilities)
        {
            if (ModelState.IsValid)
            {
                cottagesFacilities.Id = Guid.NewGuid();
                _context.Add(cottagesFacilities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottagesFacilities);
        }

        // GET: CottagesFacilities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesFacilities = await _context.CottagesFacilities.FindAsync(id);
            if (cottagesFacilities == null)
            {
                return NotFound();
            }
            return View(cottagesFacilities);
        }

        // POST: CottagesFacilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,FacilitiesId,Id,RowVersion")] CottagesFacilities cottagesFacilities)
        {
            if (id != cottagesFacilities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottagesFacilities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottagesFacilitiesExists(cottagesFacilities.Id))
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
            return View(cottagesFacilities);
        }

        // GET: CottagesFacilities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesFacilities = await _context.CottagesFacilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesFacilities == null)
            {
                return NotFound();
            }

            return View(cottagesFacilities);
        }

        // POST: CottagesFacilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottagesFacilities = await _context.CottagesFacilities.FindAsync(id);
            _context.CottagesFacilities.Remove(cottagesFacilities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottagesFacilitiesExists(Guid id)
        {
            return _context.CottagesFacilities.Any(e => e.Id == id);
        }
    }
}
