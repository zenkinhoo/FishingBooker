using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Hooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;

namespace Hooking.Controllers
{
    public class AdventureFavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAdventureService _adventureService;


        public AdventureFavoritesController(ApplicationDbContext context, 
            IAdventureService adventureService, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _adventureService = adventureService;
            _userManager = userManager;
        }

        // GET: AdventureFavorites
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdventureFavorites.ToListAsync());
        }

        // GET: AdventureFavorites/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFavorites = await _context.AdventureFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFavorites == null)
            {
                return NotFound();
            }

            return View(adventureFavorites);
        }

        // GET: AdventureFavorites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureFavorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/AdventureFavorites/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Id,RowVersion")] AdventureFavorites adventureFavorites)
        {
            if (!AdventureExists(id.ToString()))
            {
                if (ModelState.IsValid)
                {
                    IdentityUser iUser = await _userManager.GetUserAsync(User);
                    bool isSubscribed = _adventureService.Subscribe(id, adventureFavorites, iUser);
                    if(isSubscribed)
                    {
                        return RedirectToAction("Index", "Instructors");
                    }
                    System.Diagnostics.Debug.WriteLine("bacam exception");
                    return RedirectToAction("ConcurrencyError", "Home");
                }
            }

            return RedirectToAction("Index", "Instructors");
        }

        // GET: AdventureFavorites/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFavorites = await _context.AdventureFavorites.FindAsync(id);
            if (adventureFavorites == null)
            {
                return NotFound();
            }
            return View(adventureFavorites);
        }

        // POST: AdventureFavorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,AdventureId,Id,RowVersion")] AdventureFavorites adventureFavorites)
        {
            if (id != adventureFavorites.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureFavorites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureFavoritesExists(adventureFavorites.Id))
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
            return View(adventureFavorites);
        }

        // GET: AdventureFavorites/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureFavorites = await _context.AdventureFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureFavorites == null)
            {
                return NotFound();
            }

            return View(adventureFavorites);
        }
        private bool AdventureExists(String advId)
        {

            List<AdventureFavorites> advFavs = _context.AdventureFavorites.ToList();
            foreach (AdventureFavorites advFav in advFavs)
            {
                if (advFav.AdventureId == advId)
                    return true;
            }
            return false;

        }
        // POST: AdventureFavorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureFavorites = await _context.AdventureFavorites.FindAsync(id);
            _context.AdventureFavorites.Remove(adventureFavorites);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Instructors");
        }

        private bool AdventureFavoritesExists(Guid id)
        {
            return _context.AdventureFavorites.Any(e => e.Id == id);
        }
    }
}
