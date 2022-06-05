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
    public class SystemOptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemOptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemOptions
        public async Task<IActionResult> Index()
        {
            return View(await _context.SystemOptions.ToListAsync());
        }

        // GET: SystemOptions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemOptions = await _context.SystemOptions.FindAsync(id);
            if (systemOptions == null)
            {
                return NotFound();
            }
            return View(systemOptions);
        }

        // POST: SystemOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OptionName,OptionValue,Id,RowVersion")] SystemOptions systemOptionsToUpdate)
        {
            if (id != systemOptionsToUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var systemOptions = await _context.SystemOptions.FindAsync(id);
                    _context.Entry(systemOptions).CurrentValues.SetValues(systemOptionsToUpdate);
                    _context.Update(systemOptions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemOptionsExists(systemOptionsToUpdate.Id))
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
            return View(systemOptionsToUpdate);
        }

        // POST: SystemOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var systemOptions = await _context.SystemOptions.FindAsync(id);
            _context.SystemOptions.Remove(systemOptions);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemOptionsExists(Guid id)
        {
            return _context.SystemOptions.Any(e => e.Id == id);
        }
    }
}
