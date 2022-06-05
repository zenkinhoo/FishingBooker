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
    public class FilteredBoatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FilteredBoatsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        private bool isBoatAvailable(DateTime StartDate, DateTime EndDate, BoatNotAvailablePeriod btNotAvailable)
        {
            if ((btNotAvailable.StartTime >= StartDate && btNotAvailable.StartTime <= EndDate) && btNotAvailable.EndTime >= EndDate)
            {
                return false;

            }
            else if ((btNotAvailable.EndTime >= StartDate && btNotAvailable.EndTime <= EndDate) && btNotAvailable.StartTime <= StartDate)
            {
                return false;

            }
            else if (btNotAvailable.StartTime <= StartDate && btNotAvailable.EndTime >= EndDate)
            {
                return false;
            }
            return true;
        }
        private static void filterBoats(double price, string City, double AverageGrade, List<Boat> helpBoats, Boat bt)
        {
            if (price != 0)
            {
                if (bt.RegularPrice > price)
                {
                    helpBoats.Remove(bt);
                }
            }
            if (City != null && City!="")
            {
                if (bt.City != City)
                {
                    helpBoats.Remove(bt);
                }
            }
            if (AverageGrade != 0)
            {
                if (bt.AverageGrade < AverageGrade)
                {
                    helpBoats.Remove(bt);
                }
            }
        }

        // GET: FilteredBoats
        public async Task<IActionResult> Index(DateTime StartDate, DateTime EndDate, double price = 0, string City = "", double AverageGrade = 0, int MaxPersonCount = 0, string sortOrder = "")
        {
            System.Diagnostics.Debug.WriteLine("startdate: " + StartDate.ToString());
            System.Diagnostics.Debug.WriteLine("enddate: " + EndDate.ToString());
            System.Diagnostics.Debug.WriteLine("cena: " + price.ToString());
            //    System.Diagnostics.Debug.WriteLine("grad: " + City.ToString());/
            System.Diagnostics.Debug.WriteLine("prosecna ocena: " + AverageGrade.ToString());
            if (StartDate <= DateTime.Now || EndDate <= DateTime.Now || StartDate > EndDate)
                return RedirectToAction("ConcurrencyError", "Home");

            List<Boat> tempBoats = await _context.Boat.ToListAsync();

            List<BoatNotAvailablePeriod> boatNotAvailablePeriods = await _context.BoatNotAvailablePeriod.ToListAsync();



            foreach (BoatNotAvailablePeriod btNotAvailable in boatNotAvailablePeriods)
            {
                if (!isBoatAvailable(StartDate, EndDate, btNotAvailable))
                {
                    Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(btNotAvailable.BoatId)).FirstOrDefault();
                    if (tempBoats.Contains(bt))
                    {
                        tempBoats.Remove(bt);
                    }
                }
            }
            List<Boat> helpBoats = new List<Boat>(tempBoats);
            //sada filtriramo po ostalim kriterijumima
            foreach (Boat bt in tempBoats)
            {
                filterBoats(price, City, AverageGrade, helpBoats, bt);
            }
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["PersonCount"] = MaxPersonCount;
            ViewData["Price"] = price;
            ViewData["AverageGrade"] = AverageGrade;
            ViewData["City"] = City;
            List<FilteredBoats> filteredBoats = new List<FilteredBoats>();
            foreach (Boat bt in helpBoats)
            {
                FilteredBoats filteredBoat = new FilteredBoats();
                filteredBoat.BoatId = bt.Id.ToString();
                filteredBoat.Name = bt.Name;
                filteredBoat.Price = bt.RegularPrice;
                filteredBoat.GradeCount = bt.GradeCount;
                filteredBoat.Address = bt.Address;
                filteredBoat.AverageGrade = bt.AverageGrade;
                filteredBoat.City = bt.City;
                filteredBoat.Length = bt.Length;
                filteredBoat.MaxSpeed = bt.MaxSpeed;
                filteredBoats.Add(filteredBoat);
            }
            System.Diagnostics.Debug.WriteLine("sortorder: " + sortOrder.ToString());

            if (sortOrder != "")
            {
                System.Diagnostics.Debug.WriteLine("ulazim u switch: ");
                switch (sortOrder)
                {
                    case "Name":
                        System.Diagnostics.Debug.WriteLine("sortiram po imenu: ");

                        filteredBoats = filteredBoats.OrderBy(b => b.Name).ToList();
                        break;
                    case "Address":
                        System.Diagnostics.Debug.WriteLine("sortiram po adresi: ");

                        filteredBoats = filteredBoats.OrderBy(b => b.Address).ToList();
                        break;
                    case "City":
                        System.Diagnostics.Debug.WriteLine("sortiram po gradu: ");

                        filteredBoats = filteredBoats.OrderBy(b => b.City).ToList();
                        break;
                    case "AverageGrade":
                        System.Diagnostics.Debug.WriteLine("sortiram po oceni: ");
                        filteredBoats = filteredBoats.OrderByDescending(b => b.AverageGrade).ToList();
                        break;
                    case "Price":
                        filteredBoats = filteredBoats.OrderBy(b => b.Price).ToList();
                        break;
                }
            }

            return View(filteredBoats);
        }

        // GET: FilteredBoats/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredBoats = await _context.FilteredBoats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredBoats == null)
            {
                return NotFound();
            }

            return View(filteredBoats);
        }

        // GET: FilteredBoats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilteredBoats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,Name,Address,City,Price,Length,MaxSpeed,AverageGrade,GradeCount,Id,RowVersion")] FilteredBoats filteredBoats)
        {
            if (ModelState.IsValid)
            {
                filteredBoats.Id = Guid.NewGuid();
                _context.Add(filteredBoats);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filteredBoats);
        }

        // GET: FilteredBoats/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredBoats = await _context.FilteredBoats.FindAsync(id);
            if (filteredBoats == null)
            {
                return NotFound();
            }
            return View(filteredBoats);
        }

        // POST: FilteredBoats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,Name,Address,City,Price,Length,MaxSpeed,AverageGrade,GradeCount,Id,RowVersion")] FilteredBoats filteredBoats)
        {
            if (id != filteredBoats.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filteredBoats);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilteredBoatsExists(filteredBoats.Id))
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
            return View(filteredBoats);
        }

        // GET: FilteredBoats/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredBoats = await _context.FilteredBoats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredBoats == null)
            {
                return NotFound();
            }

            return View(filteredBoats);
        }

        // POST: FilteredBoats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var filteredBoats = await _context.FilteredBoats.FindAsync(id);
            _context.FilteredBoats.Remove(filteredBoats);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilteredBoatsExists(Guid id)
        {
            return _context.FilteredBoats.Any(e => e.Id == id);
        }
    }
}
