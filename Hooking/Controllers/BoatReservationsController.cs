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
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Hooking.Controllers
{

    public class BoatReservationsController : Controller
    {
        public static object LockObjectState = new object();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public BoatReservationsController(ApplicationDbContext context,
                                            UserManager<IdentityUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            using (StreamReader reader = new StreamReader("./Data/emailCredentials.json"))
            {
                string json = reader.ReadToEnd();
                _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
            }
        }

        // GET: BoatReservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatReservation.ToListAsync());
        }
        public async Task<IActionResult> BoatReservationsHistory(string sortOrder = "")
        {

            var user = await _userManager.GetUserAsync(User);
            var reservations = await _context.BoatReservation.Where(m => m.UserDetailsId == user.Id).ToListAsync();

            List<BoatReservation> boatReservations = await _context.BoatReservation.ToListAsync();


            ViewData["StartDate"] = String.IsNullOrEmpty(sortOrder) ? "StartDate" : "";
            ViewData["EndDate"] = String.IsNullOrEmpty(sortOrder) ? "EndDate" : "";
            ViewData["Price"] = String.IsNullOrEmpty(sortOrder) ? "Price" : "";

            var bt = from b in boatReservations
                      select b;
            switch (sortOrder)
            {
                case "StartDate":
                    bt = bt.OrderBy(b => b.StartDate);
                    break;
                case "Address":
                    bt = bt.OrderBy(b => b.EndDate);
                    break;
                case "City":
                    bt = bt.OrderBy(b => b.Price);
                    break;
            }
            return View(bt);
        }
        // GET: BoatReservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservation = await _context.BoatReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            Guid boatId = Guid.Parse(boatReservation.BoatId);
            Guid userId = Guid.Parse(boatReservation.UserDetailsId);
            var boat = _context.Boat.Where(m => m.Id == boatId).FirstOrDefault();
            Guid userDetailsId = Guid.Parse(boatReservation.UserDetailsId);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == boatReservation.UserDetailsId).FirstOrDefault<UserDetails>();
            Guid identityUserId = Guid.Parse(userDetails.IdentityUserId);
            var identityUser = _context.Users.Where(m => m.Id == userDetails.IdentityUserId).FirstOrDefault();
            string email = identityUser.Email;
            string phoneNumber = identityUser.PhoneNumber;

            Boat bt = _context.Boat.Where(m => m.Id == boatId).FirstOrDefault();
            BoatOwner btOwner = _context.BoatOwner.Where(m => m.Id == Guid.Parse(bt.BoatOwnerId)).FirstOrDefault();
            UserDetails userOwner = _context.UserDetails.Where(m => m.Id == Guid.Parse(btOwner.UserDetailsId)).FirstOrDefault();
            if (boatReservation == null)
            {
                return NotFound();
            }
            ViewData["Boat"] = boat;
            ViewData["UserDetails"] = userDetails;
            ViewData["UserOwner"] = userOwner;
            ViewData["Email"] = email;
            ViewData["PhoneNumber"] = phoneNumber;
            return View(boatReservation);
        }

        // GET: BoatReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,UserDetailsId,StartDate,EndDate,Price,PersonCount,Id,RowVersion")] BoatReservation boatReservation)
        {
            if (ModelState.IsValid)
            {
                boatReservation.Id = Guid.NewGuid();
                _context.Add(boatReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatReservation);
        }
        public IActionResult CreateView(Guid id, Guid cId)
        {
            return View();
        }
        [HttpPost("/BoatReservations/CreateView/{id}/{cId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateView(Guid id, Guid cId, [Bind("StartDate,EndDate,PersonCount,Id,RowVersion")] BoatReservation boatReservation)
        {
            if (ModelState.IsValid)
            {
                boatReservation.Id = Guid.NewGuid();
                var boat = await _context.Boat
                     .FirstOrDefaultAsync(m => m.Id == cId);
                double numberOfDays = (boatReservation.EndDate - boatReservation.StartDate).TotalDays;
                boatReservation.Price = numberOfDays * boat.RegularPrice;
                boatReservation.UserDetailsId = id.ToString();
                boatReservation.BoatId = cId.ToString();
                boatReservation.IsReviewed = false;
                if(await IsPossible(boatReservation))
                {
                    await CreateReservation(id, boatReservation);
                } else
                {
                    return RedirectToAction("ConcurrencyError", "Home");
                }
                return RedirectToPage("/Account/Manage/BoatReservations", new { area = "Identity" });
            }
            return View(boatReservation);
        }
        public async Task<bool> CreateReservation(Guid id, BoatReservation boatReservation)
        {
            if(await IsPossible(boatReservation))
            {
                _context.Add(boatReservation);
                await _context.SaveChangesAsync();
                BoatNotAvailablePeriod boatNotAvailablePeriod = new BoatNotAvailablePeriod();
                boatNotAvailablePeriod.Id = Guid.NewGuid();
                boatNotAvailablePeriod.BoatId = boatReservation.BoatId;
                boatNotAvailablePeriod.StartTime = boatReservation.StartDate;
                boatNotAvailablePeriod.EndTime = boatReservation.EndDate;
                _context.Add(boatNotAvailablePeriod);
                await _context.SaveChangesAsync();
                string userId = id.ToString();
                UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == userId).FirstOrDefault<UserDetails>();
                var user = await _context.Users.FindAsync(userDetails.IdentityUserId);

                await _emailSender.SendEmailAsync(user.Email, "Obaveštenje o rezervaciji",
                           $"Poštovani,<br><br>Potvrđujemo Vam rezervaciju koju ste napravili u dogovoru sa vlasnikom broda na kom trenutno boravite!");

                return true;
            }
            return false;
        }
        public async Task<bool> IsPossible(BoatReservation boatReservation)
        {
            Guid boatId = Guid.Parse(boatReservation.BoatId);
            Boat boat = await _context.Boat.FirstOrDefaultAsync(m => m.Id == boatId);
            List<BoatReservation> boatReservations = await _context.BoatReservation.Where(m => m.BoatId == boatReservation.BoatId).ToListAsync();
            foreach (BoatReservation boatReservationTemp in boatReservations)
            {
                if (IsOverlapping(boatReservation.StartDate, boatReservation.EndDate, boatReservationTemp.StartDate, boatReservationTemp.EndDate))
                {
                    return false;
                }
            }
            List<BoatSpecialOffer> boatSpecialOffers = await _context.BoatSpecialOffer.Where(m => m.BoatId == boatReservation.BoatId).ToListAsync();
            foreach(BoatSpecialOffer boatSpecialOffer in boatSpecialOffers)
            {
                if(IsOverlapping(boatReservation.StartDate,boatReservation.EndDate,boatSpecialOffer.StartDate,boatSpecialOffer.EndDate))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.BoatReservation.Local)
            {
                if (reservation.BoatId == boatReservation.BoatId)
                {
                    if (IsOverlapping(reservation.StartDate, reservation.EndDate, boatReservation.StartDate, boatReservation.EndDate))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            if (start1 > end1)
                return true;

            if (start2 > end2)
                return true;

            return ((end1 < start2 && start1 < start2) ||
                        (end2 < start1 && start2 < start1));


        }

        // GET: BoatReservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservation = await _context.BoatReservation.FindAsync(id);
            if (boatReservation == null)
            {
                return NotFound();
            }
            return View(boatReservation);
        }

        // POST: BoatReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoatId,UserDetailsId,StartDate,EndDate,Price,PersonCount,Id,RowVersion")] BoatReservation boatReservation)
        {
            if (id != boatReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatReservationExists(boatReservation.Id))
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
            return View(boatReservation);
        }

        // GET: BoatReservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatReservation = await _context.BoatReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatReservation == null)
            {
                return NotFound();
            }
            return View(boatReservation);
        }
        private List<BoatNotAvailablePeriod> findPeriodToFree(BoatReservation boatReservation)
        {
            List<BoatNotAvailablePeriod> boatNotAvailablePeriods = new List<BoatNotAvailablePeriod>();
            List<BoatNotAvailablePeriod> allBoatNotAvailablePeriods = _context.BoatNotAvailablePeriod.ToList();
            foreach (BoatNotAvailablePeriod boatNotAvailablePeriod in allBoatNotAvailablePeriods)
            {
                isPeriodEqual(boatReservation, boatNotAvailablePeriods, boatNotAvailablePeriod);
            }

            return boatNotAvailablePeriods;
        }

        private static void isPeriodEqual(BoatReservation boatReservation, List<BoatNotAvailablePeriod> boatNotAvailablePeriods, BoatNotAvailablePeriod boatNotAvailablePeriod)
        {
            if (boatNotAvailablePeriod.BoatId == boatReservation.BoatId && boatNotAvailablePeriod.StartTime == boatReservation.StartDate && boatNotAvailablePeriod.EndTime == boatReservation.EndDate)
            {
                boatNotAvailablePeriods.Add(boatNotAvailablePeriod);
            }
        }
        // POST: BoatReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatReservation = await _context.BoatReservation.FindAsync(id);
            List<BoatNotAvailablePeriod> boatNotAvailablePeriodsToFree = findPeriodToFree(boatReservation);

            _context.BoatReservation.Remove(boatReservation);
            await _context.SaveChangesAsync();
            foreach (BoatNotAvailablePeriod boatNotAvailablePeriod in boatNotAvailablePeriodsToFree)
            {
                _context.BoatNotAvailablePeriod.Remove(boatNotAvailablePeriod);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Boats");

           // return RedirectToAction(nameof(Index));
        }

        private bool BoatReservationExists(Guid id)
        {
            return _context.BoatReservation.Any(e => e.Id == id);
        }

        [HttpGet("/BoatReservations/BoatFiltering")]
        public async Task<IActionResult> BoatFiltering()
        {
            var boats = await _context.BoatReservation.FirstOrDefaultAsync();
            return View(boats);
        }
        private bool isAvailable(DateTime StartDate1, DateTime EndDate1, DateTime StartDate2, DateTime EndDate2)
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


        private bool isAlreadyReserved(BoatReservation boatReservation)
        {
            List<BoatReservation> btReservations = _context.BoatReservation.ToList();
            //ovde vrsimo standardnu proveru da li takva rezervacija vikendice vec postoji u bazi
            foreach (BoatReservation btReservation in btReservations)
            {
                if (boatReservation.BoatId == btReservation.BoatId)
                {
                    if (!isAvailable(boatReservation.StartDate, boatReservation.EndDate, btReservation.StartDate, btReservation.EndDate))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpGet("/BoatReservations/BoatReservationFinished")]
        public async Task<IActionResult> BoatReservationFinished(String BoatId, DateTime StartDate, DateTime EndDate, Double Price, int PersonCount)
        {


                BoatReservation boatReservation = new BoatReservation();
            // System.Diagnostics.Debug.WriteLine(CottageId.ToString());
            var user = await _userManager.GetUserAsync(User);

            Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(BoatId)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                boatReservation.Id = Guid.NewGuid();
                boatReservation.IsReviewed = false;
                boatReservation.BoatId = BoatId;
                boatReservation.StartDate = StartDate;
                boatReservation.EndDate = EndDate;
                boatReservation.Price = Price;
                boatReservation.PersonCount = PersonCount;
                boatReservation.UserDetailsId = user.Id.ToString();

                lock (LockObjectState)
                {
                    Debug.WriteLine("usao u lock");
                    if (isAlreadyReserved(boatReservation))
                    {
                        return RedirectToAction("BoatAlreadyReserved", "Home");
                    }
                    _context.Add(boatReservation);
                    _context.SaveChanges();
                }

                Debug.WriteLine("sacuvao ssam rezervaciju broda");

                BoatNotAvailablePeriod boatNotAvailablePeriod = new BoatNotAvailablePeriod();
                boatNotAvailablePeriod.Id = Guid.NewGuid();
                boatNotAvailablePeriod.BoatId = BoatId;
                boatNotAvailablePeriod.StartTime = StartDate;
                boatNotAvailablePeriod.EndTime = EndDate;
                _context.Add(boatNotAvailablePeriod);
                await _context.SaveChangesAsync();
                Debug.WriteLine("sacuvao ssam boat not available period");

                await _emailSender.SendEmailAsync(user.Email.ToString(), "Uspesno ste rezervisali brod", $"Uspesno ste rezervisali brod '{bt.Name}' .");


                        //   transaction.Commit();
                return RedirectToAction("Index", "Boats");
                        

             }





                  return View(boatReservation);
          //  }


        }
    }
}
