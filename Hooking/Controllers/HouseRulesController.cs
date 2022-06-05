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
    public class HouseRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HouseRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HouseRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.HouseRules.ToListAsync());
        }

        // GET: HouseRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseRules = await _context.HouseRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseRules == null)
            {
                return NotFound();
            }

            return View(houseRules);
        }

        // GET: HouseRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HouseRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("HouseRules/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("PetFriendly,NonSmoking,CheckInTime,CheckOutTime,AgeRestriction,Id,RowVersion")] HouseRules houseRules)
        {
            if (ModelState.IsValid)
            {
                houseRules.Id = Guid.NewGuid();
                _context.Add(houseRules);
                await _context.SaveChangesAsync();
                CottagesHouseRules cottagesHouseRules = new CottagesHouseRules();
                cottagesHouseRules.Id = Guid.NewGuid();
                cottagesHouseRules.CottageId = id.ToString();
                cottagesHouseRules.HouseRulesId = houseRules.Id.ToString();
                _context.Add(cottagesHouseRules);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "CancelationPolicies", new { id = cottagesHouseRules.CottageId});
            }
            return View(houseRules);
        }

        // GET: HouseRules/Edit/5
        
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseRules = await _context.HouseRules.FindAsync(id);
            if (houseRules == null)
            {
                return NotFound();
            }
            return View(houseRules);
        }

        // POST: HouseRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/HouseRules/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PetFriendly,NonSmoking,CheckInTime,CheckOutTime,AgeRestriction,Id,RowVersion")] HouseRules houseRules)
        {
            if (id != houseRules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var houseRulesTmp = await _context.HouseRules.FindAsync(id);
                    houseRulesTmp.PetFriendly = houseRules.PetFriendly;
                    houseRulesTmp.NonSmoking = houseRules.NonSmoking;
                    houseRulesTmp.CheckInTime = houseRules.CheckInTime;
                    houseRulesTmp.CheckOutTime = houseRules.CheckOutTime;
                    houseRulesTmp.AgeRestriction = houseRules.AgeRestriction;
                    _context.Update(houseRulesTmp);
                    await _context.SaveChangesAsync();
                    var houseRulesId = houseRulesTmp.Id.ToString();
                    var cottageHouseRules = _context.CottagesHouseRules.Where(m => m.HouseRulesId == houseRulesId).FirstOrDefault<CottagesHouseRules>();
                    Guid cottageId = Guid.Parse(cottageHouseRules.CottageId);
                    var cottage = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault<Cottage>();
                    return RedirectToAction("MyCottage", "Cottages", new { id = cottage.Id});
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseRulesExists(houseRules.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
            return View(houseRules);
        }

        // GET: HouseRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseRules = await _context.HouseRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseRules == null)
            {
                return NotFound();
            }

            return View(houseRules);
        }

        // POST: HouseRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var houseRules = await _context.HouseRules.FindAsync(id);
            _context.HouseRules.Remove(houseRules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseRulesExists(Guid id)
        {
            return _context.HouseRules.Any(e => e.Id == id);
        }
    }
}
