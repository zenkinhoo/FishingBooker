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
using Nito.AsyncEx.Synchronous;

namespace Hooking.Controllers
{
    public class InstructorNotAvailablePeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Instructor instructor;
        public InstructorNotAvailablePeriodsController(ApplicationDbContext context,
                                                        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        
        public async Task<IActionResult> Index()
        {
            /*_context.Add(new AdventureReservation
            {
                AdventureRealisationId = "a630b7b3-1cc1-4d6c-8f76-639e711a4911",
                IsReviewed = false,
                UserDetailsId = "276b5299-c194-4048-f33b-08d9ba8dadfb"
            });
            _context.SaveChangesAsync().WaitAndUnwrapException();*/

            var user = await _userManager.GetUserAsync(User);

            Guid userId = Guid.Parse(user.Id);
            System.Diagnostics.Debug.WriteLine(userId);
            UserDetails userDetails = await _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefaultAsync<UserDetails>();
            var userDetailsId = userDetails.Id.ToString();
            instructor = await _context.Instructor.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefaultAsync<Instructor>();
            string instructorId = instructor.Id.ToString();
          
            List<InstructorNotAvailablePeriod> instructorNotAvailablePeriods = await _context.InstructorNotAvailablePeriod.Where(m => m.InstructorId == instructorId).ToListAsync<InstructorNotAvailablePeriod>();

            int i = 0;
            
            string codeForFront = "[";
            foreach (var notAvailable in instructorNotAvailablePeriods)
            {
                if (i++ > 0)
                {
                    codeForFront += ",";
                }

                codeForFront += "{ title: '" + notAvailable.title + "', allDay : '" + true + "', start: '" +
                                notAvailable.StartTime.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "', " +
                                "end: '" + notAvailable.EndTime.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "'}\n";
            }

            var adventures = _context.Adventure.Where(a => a.InstructorId == instructorId).ToList();

            foreach (Adventure adventure in adventures)
            {
                var offers = _context.AdventureSpecialOffer.Where(o => o.AdventureId == adventure.Id.ToString()).ToList();
                foreach (AdventureSpecialOffer offer in offers)
                {
                    codeForFront += ",";
                    codeForFront += "{ title: '" + adventure.Name + "', allDay : '" + true + "', start: '" +
                                offer.StartDate.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "', " +
                                "end: '" + offer.StartDate.AddDays(offer.Duration).ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "'}\n";
                }

                var realizations = _context.AdventureRealisation.Where(r => r.AdventureId == adventure.Id.ToString()).ToList();
                foreach (AdventureRealisation realization in realizations)
                {
                    var reservations = _context.AdventureReservation.Where(r => r.AdventureRealisationId == realization.Id.ToString()).ToList();
                    foreach (AdventureReservation reservation in reservations)
                    {
                        codeForFront += ",";
                        codeForFront += "{ title: '" + adventure.Name + "', allDay : '" + true + "', url:'" + $"AdventureReservations/Details/{reservation.Id}" + "', start: '" +
                                        realization.StartDate.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "', " +
                                        "end: '" + realization.StartDate.AddDays(realization.Duration).ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss") + "'}\n";
                    }
                }
            }
            codeForFront += "]";
            codeForFront = codeForFront.Replace("‘", "").Replace("’", "");
            ViewData["codeForFront"] = codeForFront;
            ViewData["Instructor"] = instructor;
            return View();
        }

        // GET: InstructorNotAvailablePeriods/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorNotAvailablePeriod = await _context.InstructorNotAvailablePeriod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorNotAvailablePeriod == null)
            {
                return NotFound();
            }

            return View(instructorNotAvailablePeriod);
        }

        // GET: InstructorNotAvailablePeriods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InstructorNotAvailablePeriods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartTime,EndTime,Id,RowVersion")] InstructorNotAvailablePeriod instructorNotAvailablePeriod)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                Guid userId = Guid.Parse(user.Id);
                System.Diagnostics.Debug.WriteLine(userId);
                UserDetails userDetails = await _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefaultAsync<UserDetails>();
                var userDetailsId = userDetails.Id.ToString();
                instructor = await _context.Instructor.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefaultAsync<Instructor>();
                string instructorId = instructor.Id.ToString();
                instructorNotAvailablePeriod.Id = Guid.NewGuid();
                instructorNotAvailablePeriod.InstructorId = instructorId;
                instructorNotAvailablePeriod.title = "Slobodan dan";
                _context.Add(instructorNotAvailablePeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructorNotAvailablePeriod);
        }

        // GET: InstructorNotAvailablePeriods/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorNotAvailablePeriod = await _context.InstructorNotAvailablePeriod.FindAsync(id);
            if (instructorNotAvailablePeriod == null)
            {
                return NotFound();
            }
            return View(instructorNotAvailablePeriod);
        }

        // POST: InstructorNotAvailablePeriods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InstructorId,StartTime,EndTime,Id,RowVersion")] InstructorNotAvailablePeriod instructorNotAvailablePeriod)
        {
            if (id != instructorNotAvailablePeriod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructorNotAvailablePeriod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorNotAvailablePeriodExists(instructorNotAvailablePeriod.Id))
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
            return View(instructorNotAvailablePeriod);
        }

        // GET: InstructorNotAvailablePeriods/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorNotAvailablePeriod = await _context.InstructorNotAvailablePeriod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorNotAvailablePeriod == null)
            {
                return NotFound();
            }

            return View(instructorNotAvailablePeriod);
        }

        // POST: InstructorNotAvailablePeriods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var instructorNotAvailablePeriod = await _context.InstructorNotAvailablePeriod.FindAsync(id);
            _context.InstructorNotAvailablePeriod.Remove(instructorNotAvailablePeriod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorNotAvailablePeriodExists(Guid id)
        {
            return _context.InstructorNotAvailablePeriod.Any(e => e.Id == id);
        }
    }
}
