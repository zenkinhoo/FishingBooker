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
    public class BoatFavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BoatFavoritesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BoatFavorites
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatFavorites.ToListAsync());
        }

        // GET: BoatFavorites/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFavorites = await _context.BoatFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatFavorites == null)
            {
                return NotFound();
            }

            return View(boatFavorites);
        }

        // GET: BoatFavorites/Create
        public IActionResult Create()
        {
            return View();
        }

        public bool BoatExists(String boatId)
        {

            List<BoatFavorites> btFavs = _context.BoatFavorites.ToList();
            foreach (BoatFavorites btFav in btFavs)
            {
                if (btFav.BoatId == boatId)
                    return true;
            }
            return false;

        }

        // POST: BoatFavorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/BoatFavorites/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Id,RowVersion")] BoatFavorites boatFavorites)
        {
            if (!BoatExists(id.ToString()))
            {
                if (ModelState.IsValid)
                {
 
                    boatFavorites.Id = Guid.NewGuid();
                    boatFavorites.BoatId = id.ToString();
                    var user = await _userManager.GetUserAsync(User);
                    System.Diagnostics.Debug.WriteLine(user.Id);
                    boatFavorites.UserDetailsId = user.Id.ToString();

                    Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(boatFavorites.BoatId)).FirstOrDefault();
                    bt.hasSubscribers = true;
                    try
                    {
                        _context.Update(bt);
                        await _context.SaveChangesAsync();
                        _context.Add(boatFavorites);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        System.Diagnostics.Debug.WriteLine("bacam exception");
                        return RedirectToAction("ConcurrencyError", "Home");

                    }


                }
            }
            return RedirectToAction("Index", "Boats");
        }

        // GET: BoatFavorites/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFavorites = await _context.BoatFavorites.FindAsync(id);
            if (boatFavorites == null)
            {
                return NotFound();
            }
            return View(boatFavorites);
        }

        // POST: BoatFavorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,BoatId,Id,RowVersion")] BoatFavorites boatFavorites)
        {
            if (id != boatFavorites.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatFavorites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatFavoritesExists(boatFavorites.Id))
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
            return View(boatFavorites);
        }

        // GET: BoatFavorites/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatFavorites = await _context.BoatFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatFavorites == null)
            {
                return NotFound();
            }

            return View(boatFavorites);
        }

        // POST: BoatFavorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatFavorites = await _context.BoatFavorites.FindAsync(id);
            _context.BoatFavorites.Remove(boatFavorites);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Boats");
        }

        private bool BoatFavoritesExists(Guid id)
        {
            return _context.BoatFavorites.Any(e => e.Id == id);
        }
    }
}
