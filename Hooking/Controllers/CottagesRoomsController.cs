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
    public class CottagesRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CottagesRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CottagesRooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottagesRooms.ToListAsync());
        }

        // GET: CottagesRooms/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesRooms = await _context.CottagesRooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesRooms == null)
            {
                return NotFound();
            }

            return View(cottagesRooms);
        }

        // GET: CottagesRooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottagesRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,CottageRoomId,Id,RowVersion")] CottagesRooms cottagesRooms)
        {
            if (ModelState.IsValid)
            {
                cottagesRooms.Id = Guid.NewGuid();
                _context.Add(cottagesRooms);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottagesRooms);
        }

        // GET: CottagesRooms/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesRooms = await _context.CottagesRooms.FindAsync(id);
            if (cottagesRooms == null)
            {
                return NotFound();
            }
            return View(cottagesRooms);
        }

        // POST: CottagesRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,CottageRoomId,Id,RowVersion")] CottagesRooms cottagesRooms)
        {
            if (id != cottagesRooms.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottagesRooms);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottagesRoomsExists(cottagesRooms.Id))
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
            return View(cottagesRooms);
        }

        // GET: CottagesRooms/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottagesRooms = await _context.CottagesRooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottagesRooms == null)
            {
                return NotFound();
            }

            return View(cottagesRooms);
        }

        // POST: CottagesRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottagesRooms = await _context.CottagesRooms.FindAsync(id);
            _context.CottagesRooms.Remove(cottagesRooms);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottagesRoomsExists(Guid id)
        {
            return _context.CottagesRooms.Any(e => e.Id == id);
        }
    }
}
