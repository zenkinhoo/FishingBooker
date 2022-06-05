using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;

namespace Hooking.Controllers
{
    public class CottageOwnersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CottageOwnersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: CottageOwners
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageOwner.ToListAsync());
        }

        // GET: CottageOwners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwner = await _context.CottageOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageOwner == null)
            {
                return NotFound();
            }

            return View(cottageOwner);
        }

        // GET: CottageOwners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageOwners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserDetailsId,AverageGrade,GradeCount,Id,RowVersion")] CottageOwner cottageOwner)
        {
            
            if (ModelState.IsValid)
            {
               
                cottageOwner.Id = Guid.NewGuid();
                var user = await _userManager.GetUserAsync(User);
                cottageOwner.UserDetailsId = user.Id.ToString();
                cottageOwner.AverageGrade = 0;
                cottageOwner.GradeCount = 0;
                _context.Add(cottageOwner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(cottageOwner);
        }

        // GET: CottageOwners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwner = await _context.CottageOwner.FindAsync(id);
            if (cottageOwner == null)
            {
                return NotFound();
            }
            return View(cottageOwner);
        }

        // POST: CottageOwners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,AverageGrade,GradeCount,Id,RowVersion")] CottageOwner cottageOwner)
        {
            if (id != cottageOwner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageOwnerExists(cottageOwner.Id))
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
            return View(cottageOwner);
        }

        // GET: CottageOwners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageOwner = await _context.CottageOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageOwner == null)
            {
                return NotFound();
            }

            return View(cottageOwner);
        }

        // POST: CottageOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageOwner = await _context.CottageOwner.FindAsync(id);
            _context.CottageOwner.Remove(cottageOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageOwnerExists(Guid id)
        {
            return _context.CottageOwner.Any(e => e.Id == id);
        }
    }
}
