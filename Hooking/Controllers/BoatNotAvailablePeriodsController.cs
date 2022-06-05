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
    public class BoatNotAvailablePeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public BoatNotAvailablePeriodsController(ApplicationDbContext context,
                                                 UserManager<IdentityUser> userManager,
                                                 RoleManager<IdentityRole> roleManager,
                                                 SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // GET: BoatNotAvailablePeriods
        [HttpGet("/BoatNotAvailablePeriods/Index/{id}")]
        public async Task<IActionResult> Index(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }


            Boat boat = _context.Boat.Where(m => m.Id == id).FirstOrDefault();
            string boatId = boat.Id.ToString();
            List<BoatNotAvailablePeriod> boatNotAvailablePeriods = _context.BoatNotAvailablePeriod.Where(m => m.BoatId == boatId).ToList();
            string codeForFront = "[";

            int i = 0;
            string title = "Rezervacija";
            foreach (BoatNotAvailablePeriod boatNotAvailablePeriod in boatNotAvailablePeriods)
            {
                if (i++ > 0)
                {
                    codeForFront += ",";
                }



                codeForFront += "{ title: '" + title + "', allDay : '" + true + "', start: '" +
                    boatNotAvailablePeriod.StartTime.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "', " +
                    "end: '" + boatNotAvailablePeriod.EndTime.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "'}\n";
            }
            codeForFront += "]";
            codeForFront = codeForFront.Replace("‘", "").Replace("’", "");


            ViewData["Boat"] = boat;
            ViewData["codeForFront"] = codeForFront;
            return View();
        }

        // GET: BoatNotAvailablePeriods/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatNotAvailablePeriod = await _context.BoatNotAvailablePeriod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatNotAvailablePeriod == null)
            {
                return NotFound();
            }

            return View(boatNotAvailablePeriod);
        }

        // GET: BoatNotAvailablePeriods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatNotAvailablePeriods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/BoatNotAvailablePeriods/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("StartTime,EndTime,Id,RowVersion")] BoatNotAvailablePeriod boatNotAvailablePeriod)
        {
            if (ModelState.IsValid)
            {
                boatNotAvailablePeriod.Id = Guid.NewGuid();
                boatNotAvailablePeriod.BoatId = id.ToString();
                _context.Add(boatNotAvailablePeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "BoatNotAvailablePeriods", new { id = id });
            }
            return View(boatNotAvailablePeriod);
        }

        // GET: BoatNotAvailablePeriods/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatNotAvailablePeriod = await _context.BoatNotAvailablePeriod.FindAsync(id);
            if (boatNotAvailablePeriod == null)
            {
                return NotFound();
            }
            return View(boatNotAvailablePeriod);
        }

        // POST: BoatNotAvailablePeriods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,StartTime,EndTime,Id,RowVersion")] BoatNotAvailablePeriod boatNotAvailablePeriod)
        {
            if (id != boatNotAvailablePeriod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatNotAvailablePeriod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatNotAvailablePeriodExists(boatNotAvailablePeriod.Id))
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
            return View(boatNotAvailablePeriod);
        }

        // GET: BoatNotAvailablePeriods/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatNotAvailablePeriod = await _context.BoatNotAvailablePeriod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatNotAvailablePeriod == null)
            {
                return NotFound();
            }

            return View(boatNotAvailablePeriod);
        }

        // POST: BoatNotAvailablePeriods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatNotAvailablePeriod = await _context.BoatNotAvailablePeriod.FindAsync(id);
            _context.BoatNotAvailablePeriod.Remove(boatNotAvailablePeriod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatNotAvailablePeriodExists(Guid id)
        {
            return _context.BoatNotAvailablePeriod.Any(e => e.Id == id);
        }
    }
}
