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
    public class CottageFavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CottageFavoritesController(ApplicationDbContext context,
                                            UserManager<IdentityUser> userManager,
                                            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: CottageFavorites
        public async Task<IActionResult> Index()
        {
            return View(await _context.CottageFavorites.ToListAsync());
        }

        // GET: CottageFavorites/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageFavorites = await _context.CottageFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageFavorites == null)
            {
                return NotFound();
            }

            return View(cottageFavorites);
        }

        // GET: CottageFavorites/Create
        public IActionResult Create()
        {
            return View();
        }
        private bool CottageExists(String cottageId)
        {
            System.Diagnostics.Debug.WriteLine("id prosledjene vikendice" + cottageId);

            List<CottageFavorites> ctgFavs = _context.CottageFavorites.ToList();
            foreach (CottageFavorites ctgFav in ctgFavs)
            {
                System.Diagnostics.Debug.WriteLine("id ctgFav " + ctgFav.CottageId.ToString());
                if (ctgFav.CottageId == cottageId)
                    return true;
            }
            return false;

        }
        // POST: CottageFavorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/CottageFavorites/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("Id,RowVersion")] CottageFavorites cottageFavorites)
        {
            if(!CottageExists(id.ToString()))
            {
                if (ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine(id);
                    cottageFavorites.Id = Guid.NewGuid();
                    cottageFavorites.CottageId = id.ToString();
                    var user = await _userManager.GetUserAsync(User);
                    System.Diagnostics.Debug.WriteLine(user.Id);
                    cottageFavorites.UserDetailsId = user.Id.ToString();
                    Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(cottageFavorites.CottageId)).FirstOrDefault();
                    ctg.hasSubscribers = true;
                    try
                    {

                        _context.Add(cottageFavorites);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        System.Diagnostics.Debug.WriteLine("bacam exception");
                        return RedirectToAction("ConcurrencyError", "Home");

                    }

                }
            }
            return RedirectToAction("Index", "Cottages");
        }

        // GET: CottageFavorites/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageFavorites = await _context.CottageFavorites.FindAsync(id);
            if (cottageFavorites == null)
            {
                return NotFound();
            }
            return View(cottageFavorites);
        }

        // POST: CottageFavorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,CottageId,Id,RowVersion")] CottageFavorites cottageFavorites)
        {
            if (id != cottageFavorites.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageFavorites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageFavoritesExists(cottageFavorites.Id))
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
            return View(cottageFavorites);
        }

        // GET: CottageFavorites/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageFavorites = await _context.CottageFavorites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageFavorites == null)
            {
                return NotFound();
            }

            return View(cottageFavorites);
        }

        // POST: CottageFavorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageFavorites = await _context.CottageFavorites.FindAsync(id);
            _context.CottageFavorites.Remove(cottageFavorites);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cottages");
        }

        private bool CottageFavoritesExists(Guid id)
        {
            return _context.CottageFavorites.Any(e => e.Id == id);
        }
    }
}
