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
    public class FilteredInstructorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilteredInstructorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool isInstructorAvailable(DateTime StartDate, DateTime EndDate, InstructorNotAvailablePeriod insNotAvailable)
        {



            if ((insNotAvailable.StartTime >= StartDate && insNotAvailable.StartTime <= EndDate) && insNotAvailable.EndTime >= EndDate)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 1 ");

                return false;


            }
            else if ((insNotAvailable.EndTime >= StartDate && insNotAvailable.EndTime <= EndDate) && insNotAvailable.StartTime <= StartDate)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 2 ");

                return false;

            }
            else if (insNotAvailable.StartTime <= StartDate && insNotAvailable.EndTime >= EndDate)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 3 ");

                return false;
            }
            return true;
        }

        // GET: FilteredInstructors
        public async Task<IActionResult> Index(DateTime StartDate, DateTime EndDate, double price = 0, string City = "", double AverageGrade = 0, int MaxPersonCount = 0, string sortOrder = "")
        {
            List<InstructorNotAvailablePeriod> instructorNotAvailablePeriods = await _context.InstructorNotAvailablePeriod.ToListAsync();
            List<Instructor> tempInstructors = await _context.Instructor.ToListAsync();

            System.Diagnostics.Debug.WriteLine("start time prosledjeni " + StartDate.ToString());
            System.Diagnostics.Debug.WriteLine("end time prosledjeni " + EndDate.ToString());
            System.Diagnostics.Debug.WriteLine("cena " + price.ToString());
            //  System.Diagnostics.Debug.WriteLine("grad " + City.ToString());
            System.Diagnostics.Debug.WriteLine("avg grade " + AverageGrade.ToString());
            System.Diagnostics.Debug.WriteLine("mpc " + MaxPersonCount.ToString());
            if (StartDate <= DateTime.Now || EndDate <= DateTime.Now || StartDate > EndDate)
                return RedirectToAction("ConcurrencyError", "Home");
            foreach (InstructorNotAvailablePeriod insNotAvailable in instructorNotAvailablePeriods)
            {

                if (!isInstructorAvailable(StartDate, EndDate, insNotAvailable))
                {

                    Instructor ins = _context.Instructor.Where(m => m.Id == Guid.Parse(insNotAvailable.InstructorId)).FirstOrDefault();
                    if (tempInstructors.Contains(ins))
                    {
                        tempInstructors.Remove(ins);
                    }
                }

            }
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["City"] = City;
            ViewData["MaxPersonCount"] = MaxPersonCount;
            ViewData["Price"] = price;
            ViewData["AverageGrade"] = AverageGrade;

            List<UserDetails> userIns = await _context.UserDetails.ToListAsync();

            var inses = tempInstructors;

            List<UserDetails> users = new List<UserDetails>();
            foreach (Instructor instructor in inses)
            {
                Guid guid = new Guid(instructor.UserDetailsId);
                UserDetails user = _context.UserDetails.Where(m => m.Id == guid).FirstOrDefault<UserDetails>();
                foreach (var userIn in userIns)
                {
                    if (userIn.Id == user.Id && !users.Contains(user))
                    {
                        users.Add(user);
                    }
                }
            }
            ViewData["UserInstructors"] = users;

            List<FilteredInstructors> filteredInstructors = new List<FilteredInstructors>();
            foreach (Instructor ins in tempInstructors)
            {
                UserDetails user = _context.UserDetails.Where(m => m.Id == Guid.Parse(ins.UserDetailsId)).FirstOrDefault<UserDetails>();


                FilteredInstructors filteredInstructor = new FilteredInstructors();
                filteredInstructor.instructorId = ins.Id.ToString();
                filteredInstructor.AverageGrade = ins.AverageGrade;
                filteredInstructor.GradeCount = ins.GradeCount;
                filteredInstructor.FirstName = user.FirstName;
                filteredInstructor.LastName = user.LastName;
                filteredInstructor.City= user.City;
                filteredInstructor.Address = user.Address;

                filteredInstructors.Add(filteredInstructor);
            }

            if (sortOrder != "")
            {
                switch (sortOrder)
                {
                    case "FirstName":
                        filteredInstructors = filteredInstructors.OrderBy(b => b.FirstName).ToList();
                        break;
                    case "LastName":
                        filteredInstructors = filteredInstructors.OrderBy(b => b.FirstName).ToList();
                        break;
                    case "Address":
                        filteredInstructors = filteredInstructors.OrderBy(b => b.Address).ToList();
                        break;
                    case "City":
                        filteredInstructors = filteredInstructors.OrderBy(b => b.City).ToList();
                        break;
                    case "AverageGrade":
                        filteredInstructors = filteredInstructors.OrderByDescending(b => b.AverageGrade).ToList();
                        break;

                }
            }
            return View(filteredInstructors);
        }

        // GET: FilteredInstructors/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredInstructors = await _context.FilteredInstructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredInstructors == null)
            {
                return NotFound();
            }

            return View(filteredInstructors);
        }

        // GET: FilteredInstructors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilteredInstructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Address,City,AverageGrade,GradeCount,Id,RowVersion")] FilteredInstructors filteredInstructors)
        {
            if (ModelState.IsValid)
            {
                filteredInstructors.Id = Guid.NewGuid();
                _context.Add(filteredInstructors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filteredInstructors);
        }

        // GET: FilteredInstructors/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredInstructors = await _context.FilteredInstructors.FindAsync(id);
            if (filteredInstructors == null)
            {
                return NotFound();
            }
            return View(filteredInstructors);
        }

        // POST: FilteredInstructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FirstName,LastName,Address,City,AverageGrade,GradeCount,Id,RowVersion")] FilteredInstructors filteredInstructors)
        {
            if (id != filteredInstructors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filteredInstructors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilteredInstructorsExists(filteredInstructors.Id))
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
            return View(filteredInstructors);
        }

        // GET: FilteredInstructors/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filteredInstructors = await _context.FilteredInstructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filteredInstructors == null)
            {
                return NotFound();
            }

            return View(filteredInstructors);
        }

        // POST: FilteredInstructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var filteredInstructors = await _context.FilteredInstructors.FindAsync(id);
            _context.FilteredInstructors.Remove(filteredInstructors);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilteredInstructorsExists(Guid id)
        {
            return _context.FilteredInstructors.Any(e => e.Id == id);
        }
    }
}
