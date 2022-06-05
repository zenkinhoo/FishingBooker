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
    public class PrivilegedUserRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrivilegedUserRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrivilegedUserRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.PrivilegedUserRequest.ToListAsync());
        }

        // GET: PrivilegedUserRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privilegedUserRequest = await _context.PrivilegedUserRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privilegedUserRequest == null)
            {
                return NotFound();
            }

            return View(privilegedUserRequest);
        }

        // GET: PrivilegedUserRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrivilegedUserRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserDetailsId,Type,IsApproved,Description,Id,RowVersion")] PrivilegedUserRequest privilegedUserRequest)
        {
            if (ModelState.IsValid)
            {
                privilegedUserRequest.Id = Guid.NewGuid();
                _context.Add(privilegedUserRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(privilegedUserRequest);
        }

        // GET: PrivilegedUserRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privilegedUserRequest = await _context.PrivilegedUserRequest.FindAsync(id);
            if (privilegedUserRequest == null)
            {
                return NotFound();
            }
            return View(privilegedUserRequest);
        }

        // POST: PrivilegedUserRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,Type,IsApproved,Description,Id,RowVersion")] PrivilegedUserRequest privilegedUserRequest)
        {
            if (id != privilegedUserRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(privilegedUserRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrivilegedUserRequestExists(privilegedUserRequest.Id))
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
            return View(privilegedUserRequest);
        }

        // GET: PrivilegedUserRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privilegedUserRequest = await _context.PrivilegedUserRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privilegedUserRequest == null)
            {
                return NotFound();
            }

            return View(privilegedUserRequest);
        }

        // POST: PrivilegedUserRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var privilegedUserRequest = await _context.PrivilegedUserRequest.FindAsync(id);
            _context.PrivilegedUserRequest.Remove(privilegedUserRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrivilegedUserRequestExists(Guid id)
        {
            return _context.PrivilegedUserRequest.Any(e => e.Id == id);
        }
    }
}
