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
    public class FilteredCottagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public FilteredCottagesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        private static void filterCottages(double price, string City, double AverageGrade, List<Cottage> helpCottages, Cottage ctg)
        {
            if (price != 0)
            {
                if (ctg.RegularPrice > price)
                {
                    System.Diagnostics.Debug.WriteLine("brisem zbog cene: ");

                    helpCottages.Remove(ctg);
                }
            }
            if (City != "" && City != null)
            {
                if (ctg.City != City)
                {
                    System.Diagnostics.Debug.WriteLine("brisem zbog grada: ");

                    helpCottages.Remove(ctg);
                }
            }
            if (AverageGrade != 0)
            {
                if (ctg.AverageGrade < AverageGrade)
                {
                    System.Diagnostics.Debug.WriteLine("brisem zbog proseka: ");

                    helpCottages.Remove(ctg);
                }
            }
        }
        private bool isCottageAvailable(DateTime StartDate1, DateTime EndDate1, DateTime StartDate2, DateTime EndDate2)
        {
            if ((StartDate1 >= StartDate2 && StartDate1 <= EndDate2) && EndDate1 >= EndDate2)
            {
                return false;

            }
            else if ((EndDate1 >= StartDate2 && EndDate1 <= EndDate2) && StartDate1 <= StartDate2)
            {
                return false;

            }
            else if (StartDate1 <= StartDate2 && EndDate1 >= EndDate2)
            {
                return false;
            }
            return true;

        }
        // GET: FilteredCottages
        public async Task<IActionResult> Index(DateTime StartDate, DateTime EndDate, double price = 0, string City = "", double AverageGrade = 0, int MaxPersonCount = 0,string sortOrder="")
        {
            System.Diagnostics.Debug.WriteLine("sortorder: " + sortOrder.ToString());

            System.Diagnostics.Debug.WriteLine("startdate: " + StartDate.ToString());
            System.Diagnostics.Debug.WriteLine("enddate: " + EndDate.ToString());

            System.Diagnostics.Debug.WriteLine("price: " + price.ToString());
            System.Diagnostics.Debug.WriteLine("avggrade: " + AverageGrade.ToString());
       //     System.Diagnostics.Debug.WriteLine("city: " + City.ToString());
            System.Diagnostics.Debug.WriteLine("mpc: " + MaxPersonCount.ToString());

            if (StartDate <= DateTime.Now || EndDate <= DateTime.Now || StartDate > EndDate)
                return RedirectToAction("ConcurrencyError", "Home");


            List<Cottage> tempCottages = await _context.Cottage.ToListAsync();


            List<CottageNotAvailablePeriod> cottageNotAvailablePeriods = await _context.CottageNotAvailablePeriod.ToListAsync();

            //ovde filtriramo po zauzetosti datuma
            foreach (CottageNotAvailablePeriod ctgNotAvailable in cottageNotAvailablePeriods)
            {

                if (!isCottageAvailable(ctgNotAvailable.StartTime, ctgNotAvailable.EndTime, StartDate, EndDate))
                {
                    Cottage cotg = _context.Cottage.Where(m => m.Id == Guid.Parse(ctgNotAvailable.CottageId)).FirstOrDefault();
                    if (tempCottages.Contains(cotg))
                    {
                        tempCottages.Remove(cotg);
                    }
                }
            }


            List<Cottage> helpCottages = new List<Cottage>(tempCottages);
            //sada filtriramo po ostalim kriterijumima
            foreach (Cottage cotg in tempCottages)
            {
                filterCottages(price, City, AverageGrade, helpCottages, cotg);
            }

            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["MaxPersonCount"] = MaxPersonCount;
            ViewData["Price"] = price;
            ViewData["AverageGrade"] =AverageGrade;
            ViewData["City"] = City;

            List<FilteredCottages> filteredCottages = new List<FilteredCottages>();
            foreach(Cottage cotg in helpCottages)
            {
                FilteredCottages filteredCottage = new FilteredCottages();
                filteredCottage.CottageId = cotg.Id.ToString();
                filteredCottage.Name = cotg.Name;
                filteredCottage.Price = cotg.RegularPrice;
                filteredCottage.RoomCount = cotg.RoomCount;
                filteredCottage.GradeCount = cotg.GradeCount;
                filteredCottage.Address = cotg.Address;
                filteredCottage.AverageGrade = cotg.AverageGrade;
                filteredCottage.Area = cotg.Area;
                filteredCottage.City = cotg.City;
                filteredCottages.Add(filteredCottage);
            }

            if (sortOrder!="")
            {
                switch (sortOrder)
                {
                    case "Name":
                        filteredCottages = filteredCottages.OrderBy(b => b.Name).ToList();
                        break;
                    case "Address":
                        filteredCottages = filteredCottages.OrderBy(b => b.Address).ToList();
                        break;
                    case "City":
                        filteredCottages = filteredCottages.OrderBy(b => b.City).ToList();
                        break;
                    case "AverageGrade":
                        filteredCottages = filteredCottages.OrderByDescending(b => b.AverageGrade).ToList();
                        break;
                    case "Price":
                        filteredCottages = filteredCottages.OrderBy(b => b.Price).ToList();
                        break;
                }
            }


            return View(filteredCottages);
         //   return View(await _context.FilteredCottages.ToListAsync());
        }

        // GET: FilteredCottages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredCottages = await _context.FilteredCottages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredCottages == null)
            {
                return NotFound();
            }

            return View(filteredCottages);
        }

        // GET: FilteredCottages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilteredCottages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,Name,Address,City,Price,RoomCount,Area,AverageGrade,GradeCount,Id,RowVersion")] FilteredCottages filteredCottages)
        {
            if (ModelState.IsValid)
            {
                filteredCottages.Id = Guid.NewGuid();
                _context.Add(filteredCottages);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filteredCottages);
        }

        // GET: FilteredCottages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredCottages = await _context.FilteredCottages.FindAsync(id);
            if (filteredCottages == null)
            {
                return NotFound();
            }
            return View(filteredCottages);
        }

        // POST: FilteredCottages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,Name,Address,City,Price,RoomCount,Area,AverageGrade,GradeCount,Id,RowVersion")] FilteredCottages filteredCottages)
        {
            if (id != filteredCottages.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filteredCottages);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilteredCottagesExists(filteredCottages.Id))
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
            return View(filteredCottages);
        }

        // GET: FilteredCottages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredCottages = await _context.FilteredCottages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredCottages == null)
            {
                return NotFound();
            }

            return View(filteredCottages);
        }

        // POST: FilteredCottages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var filteredCottages = await _context.FilteredCottages.FindAsync(id);
            _context.FilteredCottages.Remove(filteredCottages);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilteredCottagesExists(Guid id)
        {
            return _context.FilteredCottages.Any(e => e.Id == id);
        }
    }
}
