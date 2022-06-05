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
using System.Diagnostics;

namespace Hooking.Controllers
{

    public class CottageReservationsController : Controller
    {
        public static object LockObjectState = new object();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public Cottage cottage;
        public UserDetails userDetails;

        public CottageReservationsController(ApplicationDbContext context,
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


        // GET: CottageReservations
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            var reservations = await _context.CottageReservation.Where(m => m.UserDetailsId == user.Id).ToListAsync();

            // return View(await _context.CottageReservation.ToListAsync());
            return View(reservations);
        }

        // GET: CottageReservationsHistory
        public async Task<IActionResult> CottageReservationsHistory(string sortOrder = "")
        {

            var user = await _userManager.GetUserAsync(User);
            var reservations = await _context.CottageReservation.Where(m => m.UserDetailsId == user.Id).ToListAsync();

            List<CottageReservation> cottageReservations = await _context.CottageReservation.ToListAsync();


            ViewData["StartDate"] = String.IsNullOrEmpty(sortOrder) ? "StartDate" : "";
            ViewData["EndDate"] = String.IsNullOrEmpty(sortOrder) ? "EndDate" : "";
            ViewData["Price"] = String.IsNullOrEmpty(sortOrder) ? "Price" : "";

            var ctg = from b in cottageReservations
                      select b;
            switch (sortOrder)
            {
                case "StartDate":
                    ctg = ctg.OrderBy(b => b.StartDate);
                    break;
                case "Address":
                    ctg = ctg.OrderBy(b => b.EndDate);
                    break;
                case "City":
                    ctg = ctg.OrderBy(b => b.Price);
                    break;
            }


            // return View(await _context.CottageReservation.ToListAsync());
            return View(ctg);
        }
        [HttpGet]
        public IActionResult CreateView(Guid id, Guid cId)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateView(Guid id, Guid cId, [Bind("StartDate,EndDate,MaxPersonCount,Id,RowVersion")] CottageReservation cottageReservation)
        {
            if (ModelState.IsValid)
            {

                cottageReservation.Id = Guid.NewGuid();
                var cottage = await _context.Cottage
                     .FirstOrDefaultAsync(m => m.Id == cId);
                double numberOfDays = (cottageReservation.EndDate - cottageReservation.StartDate).TotalDays;
                cottageReservation.Price = numberOfDays * cottage.RegularPrice;
                cottageReservation.StartDate = cottageReservation.StartDate.Date;
                cottageReservation.EndDate = cottageReservation.EndDate.Date;
                cottageReservation.UserDetailsId = id.ToString();
                cottageReservation.CottageId = cId.ToString();
                cottageReservation.IsReviewed = false;
                if (await IsPossible(cottageReservation))
                {
                    await CreateReservation(id, cottageReservation);
                } else
                {
                    return RedirectToAction("ConcurrencyError", "Home");
                }

                return RedirectToPage("/Account/Manage/CottagesReservations", new { area = "Identity" });

            }
            return View();

        }
        public async Task<bool> CreateReservation(Guid id, CottageReservation cottageReservation)
        {
            if (await IsPossible(cottageReservation))
            {
                _context.Add(cottageReservation);
                await _context.SaveChangesAsync();
                CottageNotAvailablePeriod cottageNotAvailablePeriod = new CottageNotAvailablePeriod();
                cottageNotAvailablePeriod.Id = Guid.NewGuid();
                cottageNotAvailablePeriod.CottageId = cottageReservation.CottageId;
                cottageNotAvailablePeriod.StartTime = cottageReservation.StartDate;
                cottageNotAvailablePeriod.EndTime = cottageReservation.EndDate;
                _context.Add(cottageNotAvailablePeriod);
                await _context.SaveChangesAsync();
                string userId = id.ToString();
                UserDetails userDetails = _context.UserDetails.Where(m => m.Id == id).FirstOrDefault<UserDetails>();
                var user = await _context.Users.FindAsync(userDetails.IdentityUserId);

                await _emailSender.SendEmailAsync(user.Email, "Obaveštenje o rezervaciji",
                           $"Poštovani,<br><br>Potvrđujemo Vam rezervaciju koju ste napravili u dogovoru sa vlasnikom objekta gde trenutno boravite!");

                return true;
            }
            return false;
        }
        public async Task<bool> IsPossible(CottageReservation cottageReservation)
        {
            Guid cottageId = Guid.Parse(cottageReservation.CottageId);
            Cottage cottage = await _context.Cottage.FirstOrDefaultAsync(m => m.Id == cottageId);
            List<CottageReservation> cottageReservations = await _context.CottageReservation.Where(m => m.CottageId == cottageReservation.CottageId).ToListAsync();
            foreach (CottageReservation cottageReservationTemp in cottageReservations)
            {
                if (IsOverlapping(cottageReservation.StartDate, cottageReservation.EndDate, cottageReservationTemp.StartDate, cottageReservationTemp.EndDate))
                {
                    return false;
                }
            }
            List<CottageSpecialOffer> cottageSpecialOffers = await _context.CottageSpecialOffer.Where(m => m.CottageId == cottageReservation.CottageId).ToListAsync();
            foreach (CottageSpecialOffer cottageSpecialOffer in cottageSpecialOffers)
            {
                if (IsOverlapping(cottageReservation.StartDate, cottageReservation.EndDate, cottageSpecialOffer.StartDate, cottageSpecialOffer.EndDate))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.CottageReservation.Local)
            {
                if (reservation.CottageId == cottageReservation.CottageId)
                {
                    if (IsOverlapping(reservation.StartDate, reservation.EndDate, cottageReservation.StartDate, cottageReservation.EndDate))
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
        // GET: CottageReservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservation = await _context.CottageReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            Guid cottageId = Guid.Parse(cottageReservation.CottageId);
            cottage = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault<Cottage>();
            userDetails = _context.UserDetails.Where(m => m.IdentityUserId == cottageReservation.UserDetailsId).FirstOrDefault();
            Guid identityUserId = Guid.Parse(userDetails.IdentityUserId);
            var identityUser = _context.Users.Where(m => m.Id == userDetails.IdentityUserId).FirstOrDefault();
            string email = identityUser.Email;
            string phoneNumber = identityUser.PhoneNumber;
            Cottage ctg = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault();
            CottageOwner ctgOwner = _context.CottageOwner.Where(m => m.Id == Guid.Parse(ctg.CottageOwnerId)).FirstOrDefault();
            UserDetails userOwner = _context.UserDetails.Where(m => m.Id == Guid.Parse(ctgOwner.UserDetailsId)).FirstOrDefault();
            if (cottageReservation == null)
            {
                return NotFound();
            }
            ViewData["Cottage"] = cottage;
            ViewData["UserDetails"] = userDetails;
            ViewData["UserOwner"] = userOwner;
            ViewData["Email"] = email;
            ViewData["PhoneNumber"] = phoneNumber;
            return View(cottageReservation);
        }

        // GET: CottageReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CottageId,UserDetailsId,StartDate,EndDate,Price,MaxPersonCount,Id,RowVersion")] CottageReservation cottageReservation)
        {
            if (ModelState.IsValid)
            {
                cottageReservation.Id = Guid.NewGuid();
                cottageReservation.IsReviewed = false;
                _context.Add(cottageReservation);
                await _context.SaveChangesAsync();
                CottageNotAvailablePeriod cottageNotAvailablePeriod = new CottageNotAvailablePeriod();
                cottageNotAvailablePeriod.Id = Guid.NewGuid();
                cottageNotAvailablePeriod.CottageId = cottageReservation.CottageId;
                cottageNotAvailablePeriod.StartTime = cottageReservation.StartDate;
                cottageNotAvailablePeriod.EndTime = cottageReservation.EndDate;
                _context.Add(cottageNotAvailablePeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cottageReservation);
        }

        // GET: CottageReservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservation = await _context.CottageReservation.FindAsync(id);
            if (cottageReservation == null)
            {
                return NotFound();
            }
            return View(cottageReservation);
        }

        // POST: CottageReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,UserDetailsId,StartDate,EndDate,Price,MaxPersonCount,Id,RowVersion")] CottageReservation cottageReservation)
        {
            if (id != cottageReservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cottageReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageReservationExists(cottageReservation.Id))
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
            return View(cottageReservation);
        }

        // GET: CottageReservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageReservation = await _context.CottageReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageReservation == null)
            {
                return NotFound();
            }

            return View(cottageReservation);
        }

        private List<CottageNotAvailablePeriod> findPeriodToFree(CottageReservation cottageReservation)
        {
            List<CottageNotAvailablePeriod> cottageNotAvailablePeriods = new List<CottageNotAvailablePeriod>();
            List<CottageNotAvailablePeriod> allCottageNotAvailablePeriods = _context.CottageNotAvailablePeriod.ToList();
            foreach (CottageNotAvailablePeriod cottageNotAvailablePeriod in allCottageNotAvailablePeriods)
            {
                isPeriodEqual(cottageReservation, cottageNotAvailablePeriods, cottageNotAvailablePeriod);
            }

            return cottageNotAvailablePeriods;
        }

        private static void isPeriodEqual(CottageReservation cottageReservation, List<CottageNotAvailablePeriod> cottageNotAvailablePeriods, CottageNotAvailablePeriod cottageNotAvailablePeriod)
        {
            if (cottageNotAvailablePeriod.CottageId == cottageReservation.CottageId && cottageNotAvailablePeriod.StartTime == cottageReservation.StartDate && cottageNotAvailablePeriod.EndTime == cottageReservation.EndDate)
            {
                cottageNotAvailablePeriods.Add(cottageNotAvailablePeriod);
            }
        }

        // POST: CottageReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageReservation = await _context.CottageReservation.FindAsync(id);
            List<CottageNotAvailablePeriod> cottageNotAvailablePeriodsToFree = findPeriodToFree(cottageReservation);
            _context.CottageReservation.Remove(cottageReservation);
            await _context.SaveChangesAsync();
            foreach (CottageNotAvailablePeriod cottageNotAvailablePeriod in cottageNotAvailablePeriodsToFree)
            {
                _context.CottageNotAvailablePeriod.Remove(cottageNotAvailablePeriod);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cottages");

            //  return RedirectToAction(nameof(Index));
        }

        private bool CottageReservationExists(Guid id)
        {
            return _context.CottageReservation.Any(e => e.Id == id);
        }

        [HttpGet("/CottageReservations/CottageFiltering")]
        public async Task<IActionResult> CottageFiltering()
        {
            var cottages = await _context.CottageReservation.FirstOrDefaultAsync();
            return View(cottages);
        }

        private bool isAvailable(DateTime StartDate1, DateTime EndDate1, DateTime StartDate2, DateTime EndDate2)
        {
            if ((StartDate1 >= StartDate2 && StartDate1 <= EndDate2) && EndDate1 >= EndDate2)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 1");

                return false;

            }
            else if ((EndDate1 >= StartDate2 && EndDate1 <= EndDate2) && StartDate1 <= StartDate2)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 2");

                return false;

            }
            else if (StartDate1 <= StartDate2 && EndDate1 >= EndDate2)
            {
                System.Diagnostics.Debug.WriteLine("slucaj 3");

                return false;
            }
            return true;
        }



        private bool isAlreadyReserved (CottageReservation cottageReservation)
        {
            List<CottageReservation> cottageReservations = _context.CottageReservation.ToList();
            //ovde vrsimo standardnu proveru da li takva rezervacija vikendice vec postoji u bazi
            foreach (CottageReservation ctgReservation in cottageReservations)
            {
                if (cottageReservation.CottageId == ctgReservation.CottageId)
                {
                    if (!isAvailable(cottageReservation.StartDate, cottageReservation.EndDate, ctgReservation.StartDate, ctgReservation.EndDate))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

 
        [HttpGet("/CottageReservations/CottageReservationFinished")]
        public async Task<IActionResult> CottageReservationFinished(String CottageId,DateTime StartDate,DateTime EndDate,Double Price,int MaxPersonCount)
        {

            // System.Diagnostics.Debug.WriteLine(CottageId.ToString());
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                CottageReservation cottageReservation = new CottageReservation();
                    cottageReservation.Id = Guid.NewGuid();
                cottageReservation.IsReviewed = false;
                cottageReservation.CottageId = CottageId;
                cottageReservation.StartDate = StartDate;
                cottageReservation.EndDate = EndDate;
                cottageReservation.Price = Price;
                cottageReservation.MaxPersonCount = MaxPersonCount;
                cottageReservation.UserDetailsId = user.Id;

                lock (LockObjectState)
                {
                    Debug.WriteLine("usao u lock");
                    if (isAlreadyReserved(cottageReservation))
                    {
                        return RedirectToAction("CottageAlreadyReserved", "Home");
                    }
                    _context.Add(cottageReservation);
                    _context.SaveChanges();
                }
                Debug.WriteLine("sacuvao ssam rezervaciju vikendice");





                //kreiramo period zauzetosti vikendice

                CottageNotAvailablePeriod cottageNotAvailablePeriod = new CottageNotAvailablePeriod();
                cottageNotAvailablePeriod.Id = Guid.NewGuid();
                cottageNotAvailablePeriod.CottageId = CottageId;
                cottageNotAvailablePeriod.StartTime = StartDate;
                cottageNotAvailablePeriod.EndTime = EndDate;

                _context.Add(cottageNotAvailablePeriod);
                await _context.SaveChangesAsync();
                Debug.WriteLine("sacuvao ssam cottage not available period");

                //kreiramo dummy cottage rezervaciju



                //saljemo mejl
                Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(CottageId)).FirstOrDefault();
                await _emailSender.SendEmailAsync(user.Email.ToString(), "Uspesno ste rezervisali vikendicu", $"Uspesno ste rezervisali vikendicu '{ctg.Name}' .");
                return RedirectToAction("Index", "Cottages");

            }

            return RedirectToAction("Index", "Cottages");
        }
    }
}
