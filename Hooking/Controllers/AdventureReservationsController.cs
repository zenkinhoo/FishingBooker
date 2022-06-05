using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Hooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Hooking.Controllers
{
    public class AdventureReservationsController : Controller
    {
        public static object LockObjectState = new object();

        private readonly ApplicationDbContext _context;
        private readonly IAdventureService _adventureService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AdventureReservationsController(ApplicationDbContext context, 
            IAdventureService adventureService, 
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _adventureService = adventureService;
            _userManager = userManager;
            _emailSender = emailSender;
            using StreamReader reader = new StreamReader("./Data/emailCredentials.json");
            string json = reader.ReadToEnd();
            _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
        }

        // GET: AdventureReservations
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == userId);

            if (userDetails == null)
            {
                return NotFound();
            }

            return View(_adventureService.GetAdventureReservations(userDetails.Id));
        }
        // GET: AdventureReservations/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(Guid id)
        {
            return View(_context.AdventureReservation.Find(id));
        }

        // POST: AdventureReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdventureRealisationId,UserDetailsId,Id,RowVersion")] AdventureReservation adventureReservation)
        {
            if (ModelState.IsValid)
            {
                adventureReservation.Id = Guid.NewGuid();
                _context.Add(adventureReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adventureReservation);
        }
        public async Task<bool> CreateReservation(Guid id, AdventureReservation adventureReservation)
        {
            if (await IsPossible(adventureReservation))
            {
                _context.Add(adventureReservation);
                await _context.SaveChangesAsync();
                InstructorNotAvailablePeriod instructorNotAvailablePeriod = new InstructorNotAvailablePeriod();
                instructorNotAvailablePeriod.Id = Guid.NewGuid();
                AdventureRealisation adventureRealisation = _context.AdventureRealisation.Find(Guid.Parse(adventureReservation.AdventureRealisationId));
                Adventure adventure = _context.Adventure.Find(Guid.Parse(adventureRealisation.AdventureId));
                instructorNotAvailablePeriod.InstructorId = adventure.InstructorId;
                instructorNotAvailablePeriod.StartTime = adventureRealisation.StartDate;
                instructorNotAvailablePeriod.EndTime = adventureRealisation.StartDate.AddHours(adventureRealisation.Duration);
                _context.Add(instructorNotAvailablePeriod);
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
        public async Task<bool> IsPossible(AdventureReservation adventureReservation)
        {
            AdventureRealisation adventureRealisation = _context.AdventureRealisation.Find(Guid.Parse(adventureReservation.AdventureRealisationId));
            Adventure adventure = _context.Adventure.Find(Guid.Parse(adventureRealisation.AdventureId));
            string adventureId = adventure.Id.ToString();
            List<AdventureRealisation> adventureRealisations = await _context.AdventureRealisation.Where(m => m.AdventureId == adventureId).ToListAsync();
            List<AdventureReservation> adventureReservations = new List<AdventureReservation>();
            foreach (AdventureRealisation adventureRealisationTemp in adventureRealisations)
            {
                string realizationId = adventureRealisationTemp.ToString();
                List<AdventureReservation> adventureReservationsTemp = await _context.AdventureReservation.Where(m => m.AdventureRealisationId == realizationId).ToListAsync();
                adventureReservations.AddRange(adventureReservationsTemp);
            }
            foreach (AdventureReservation adventureReservationTemp in adventureReservations)
            {
                AdventureRealisation adventureRealisationTemp = _context.AdventureRealisation.Find(Guid.Parse(adventureReservationTemp.AdventureRealisationId));
                if (IsOverlapping(adventureRealisationTemp.StartDate, adventureRealisationTemp.StartDate.AddHours(adventureRealisationTemp.Duration), adventureRealisation.StartDate, adventureRealisation.StartDate.AddHours(adventureRealisation.Duration)))
                {
                    return false;
                }
            }
            List<AdventureSpecialOffer> adventureSpecialOffers = await _context.AdventureSpecialOffer.Where(m => m.AdventureId == adventureRealisation.AdventureId).ToListAsync();
            foreach (AdventureSpecialOffer adventureSpecialOfferTemp in adventureSpecialOffers)
            {
                if (IsOverlapping(adventureRealisation.StartDate, adventureRealisation.StartDate.AddHours(adventureRealisation.Duration), adventureSpecialOfferTemp.StartDate, adventureSpecialOfferTemp.StartDate.AddHours(adventureSpecialOfferTemp.Duration)))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.AdventureReservation.Local)
            {
                AdventureRealisation adventureRealisationTemp = _context.AdventureRealisation.Find(Guid.Parse(reservation.AdventureRealisationId));
                if (adventureRealisation.AdventureId == adventureRealisationTemp.AdventureId)
                {
                    if (IsOverlapping(adventureRealisationTemp.StartDate, adventureRealisationTemp.StartDate.AddHours(adventureRealisationTemp.Duration), 
                        adventureRealisation.StartDate, adventureRealisation.StartDate.AddHours(adventureRealisation.Duration)))
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

        // GET: AdventureReservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureReservation = await _context.AdventureReservation
                .FirstOrDefaultAsync(m => m.Id == id);
            var adventureRealisation = await _context.AdventureRealisation.FirstOrDefaultAsync(m => m.Id == Guid.Parse(adventureReservation.AdventureRealisationId));
            ViewData["AdventureRealisation"] = adventureRealisation;
            System.Diagnostics.Debug.WriteLine(adventureRealisation.StartDate.ToString());
            if (adventureReservation == null)
            {
                return NotFound();
            }

            return View(adventureReservation);
        }
        [HttpGet]
        public IActionResult CreateView(Guid id, Guid cId)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateView(Guid id, Guid cId, [Bind("Id,RowVersion")] AdventureReservation adventureReservation)
        {
            if (ModelState.IsValid)
            {
                adventureReservation.Id = Guid.NewGuid();
                adventureReservation.AdventureRealisationId = cId.ToString();
                adventureReservation.UserDetailsId = id.ToString();
                adventureReservation.IsReviewed = false;
                if(await IsPossible(adventureReservation))
                {
                    await CreateReservation(id, adventureReservation);
                } else
                {
                    return RedirectToAction("ConcurrencyError", "Home");
                }

                return Redirect("/Adventures/Index");
            }
            return Redirect("/Adventures/Index");

        }
        public IActionResult AdventureReservationHistory()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == userId);

            if (userDetails == null)
            {
                return NotFound();
            }

            return View(_adventureService.GetAdventureReservationsHistory(userDetails.Id));
        }

        private List<InstructorNotAvailablePeriod> findPeriodToFree(AdventureReservation adventureReservation)
        {
            List<InstructorNotAvailablePeriod> instructorNotAvailablePeriods = new List<InstructorNotAvailablePeriod>();
            List<InstructorNotAvailablePeriod> allInstructorNotAvailablePeriods = _context.InstructorNotAvailablePeriod.ToList();
            foreach (InstructorNotAvailablePeriod instructorNotAvailablePeriod in allInstructorNotAvailablePeriods)
            {
                isPeriodEqual(adventureReservation, instructorNotAvailablePeriods, instructorNotAvailablePeriod);
            }

            return instructorNotAvailablePeriods;
        }

        private  void isPeriodEqual(AdventureReservation adventureReservation, List<InstructorNotAvailablePeriod> instructorNotAvailablePeriods, InstructorNotAvailablePeriod instructorNotAvailablePeriod)
        {
            AdventureRealisation adventureRealisation = _context.AdventureRealisation.Where(m => m.Id == Guid.Parse(adventureReservation.AdventureRealisationId)).FirstOrDefault();
            Adventure adventure = _context.Adventure.Where(m => m.Id == Guid.Parse(adventureRealisation.AdventureId)).FirstOrDefault();

            if (instructorNotAvailablePeriod.InstructorId ==  adventure.InstructorId && instructorNotAvailablePeriod.StartTime == adventureRealisation.StartDate && instructorNotAvailablePeriod.EndTime == adventureRealisation.StartDate.AddHours(adventureRealisation.Duration))
            {
                instructorNotAvailablePeriods.Add(instructorNotAvailablePeriod);
            }
        }

        // POST: AdventureReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureReservation = await _context.AdventureReservation.FindAsync(id);
            List<InstructorNotAvailablePeriod> instructorNotAvailablePeriodsToFree = findPeriodToFree(adventureReservation);

            _context.AdventureReservation.Remove(adventureReservation);
            await _context.SaveChangesAsync();

            foreach (InstructorNotAvailablePeriod instructorNotAvailablePeriod in instructorNotAvailablePeriodsToFree)
            {
                _context.InstructorNotAvailablePeriod.Remove(instructorNotAvailablePeriod);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Instructors");

            //return RedirectToAction(nameof(Index));
        }


        [HttpGet("/AdventureReservations/InstructorFiltering")]
        public async Task<IActionResult> InstructorFiltering()
        {
            var ins = await _context.InstructorNotAvailablePeriod.FirstOrDefaultAsync();
            return View(ins);
        }

        private string adventureRealisationExists(AdventureRealisation adventureRealisation)
        {

            Debug.WriteLine("prosledjeni id avanture " + adventureRealisation.AdventureId.ToString());
            Debug.WriteLine("prosledjeni startdate " + adventureRealisation.StartDate.ToString());

            Debug.WriteLine("prosledjeni duration " + adventureRealisation.Duration.ToString());

            Debug.WriteLine("prosledjeni price " + adventureRealisation.Price.ToString());
            foreach (AdventureRealisation advRealisation in _context.AdventureRealisation.ToList())
            {
                if(adventureRealisation.AdventureId==advRealisation.AdventureId&&
                    adventureRealisation.StartDate==advRealisation.StartDate && 
                    adventureRealisation.Duration == advRealisation.Duration &&
                    adventureRealisation.Price==advRealisation.Price)
                     {
                    Debug.WriteLine("pronasao sam reazliaciju s id-om" + advRealisation.Id.ToString());

                    return advRealisation.Id.ToString();
                     }
            }
            Debug.WriteLine("vracam null");
            return "";
        }

        [HttpGet("/AdventureReservations/AdventureFastReservationFinished")]
        public async Task<IActionResult> AdventureFastReservationFinished(String AdventureId, DateTime startDate, double Duration, double Price)
        {


            AdventureReservation adventureReservation = new AdventureReservation();

            if (ModelState.IsValid)
            {
                AdventureRealisation adventureRealisation = new AdventureRealisation();
                adventureRealisation.Id = Guid.NewGuid();
                adventureRealisation.AdventureId = AdventureId;
                adventureRealisation.StartDate = startDate;
                adventureRealisation.Duration = Duration;
                adventureRealisation.Price = Price;

                if(adventureRealisationExists(adventureRealisation)!="")
                {
                    adventureRealisation.Id = (Guid.Parse(adventureRealisationExists(adventureRealisation)));
                }
                else
                {
                    adventureRealisation.Id = Guid.NewGuid();
                    Debug.WriteLine("realizacija ne postoji, pravim je");
                    _context.Add(adventureRealisation);
                    await _context.SaveChangesAsync();
                }
                // Debug.WriteLine("id realizacije je" + adventureRealisation.Id.ToString());

                Adventure adventure = _context.Adventure.Where(m => m.Id == Guid.Parse(AdventureId)).FirstOrDefault();
                var user = await _userManager.GetUserAsync(User);
                adventureReservation.AdventureRealisationId = adventureRealisation.Id.ToString();
                adventureReservation.UserDetailsId = user.Id;
                adventureReservation.IsReviewed = false;

                lock (LockObjectState)
                {
                    Debug.WriteLine("usao u lock");
                    if (isAlreadyReserved(adventureReservation))
                    {
                        return RedirectToAction("AdventureAlreadyReserved", "Home");
                    }
                    _context.Add(adventureReservation);
                    _context.SaveChanges();
                }

                await _emailSender.SendEmailAsync(user.Email.ToString(), "Uspesno ste rezervisali avanturu", $"Uspesno ste rezervisali avanturu '{adventure.Name}' .");

                InstructorNotAvailablePeriod instructorNotAvailablePeriod = new InstructorNotAvailablePeriod();
                instructorNotAvailablePeriod.Id = Guid.NewGuid();
                instructorNotAvailablePeriod.InstructorId = adventure.InstructorId;
                instructorNotAvailablePeriod.StartTime = adventureRealisation.StartDate;
                instructorNotAvailablePeriod.EndTime = adventureRealisation.StartDate.AddHours(adventureRealisation.Duration);
                _context.InstructorNotAvailablePeriod.Add(instructorNotAvailablePeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Instructors");

            }

            return View(adventureReservation);
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


        private bool isAlreadyReserved(AdventureReservation adventureReservation)
        {
            //cottage reservations in local buffer
            AdventureRealisation adventureRealisation = _context.AdventureRealisation.Where(m=>m.Id == Guid.Parse(adventureReservation.AdventureRealisationId)).FirstOrDefault();
            System.Diagnostics.Debug.WriteLine("id prosledjene realizacije " + adventureRealisation.Id.ToString());
            List<AdventureReservation> advReservations = _context.AdventureReservation.ToList();


            //ako 2 klijenta pokusaju rezervaciju na istu vikendicu u isto vreme provericemo da li u lokalnom
            //bufferu postoji ta rezervacija u to vreme i ako postoji jedan od njih nece moci da rezervise
            foreach (AdventureReservation advReservation in advReservations)
            {
                AdventureRealisation adventureRealisationTemp = _context.AdventureRealisation.Find(Guid.Parse(advReservation.AdventureRealisationId));
                System.Diagnostics.Debug.WriteLine("id trenutne realizacije " + adventureRealisationTemp.Id.ToString());

                if (adventureRealisation.AdventureId == adventureRealisationTemp.AdventureId)
                {
                    if (!isAvailable(adventureRealisation.StartDate, adventureRealisation.StartDate.AddHours(adventureRealisation.Duration),
                        adventureRealisationTemp.StartDate, adventureRealisationTemp.StartDate.AddHours(adventureRealisation.Duration)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpGet("/AdventureReservations/AdventureReservationFinished")]
        public async Task<IActionResult> AdventureReservationFinished(String AdventureId,String AdventureRealisationId)
        {

            AdventureReservation adventureReservation = new AdventureReservation();
            Adventure adventure = _context.Adventure.Where(m => m.Id == Guid.Parse(AdventureId)).FirstOrDefault();
            AdventureRealisation adventureRealisation = _context.AdventureRealisation.Where(m => m.Id == Guid.Parse(AdventureRealisationId)).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                adventureReservation.AdventureRealisationId = AdventureRealisationId;
                adventureReservation.UserDetailsId = user.Id;
                adventureReservation.IsReviewed = false;


                lock (LockObjectState)
                {
                    Debug.WriteLine("usao u lock");
                    if (isAlreadyReserved(adventureReservation))
                    {
                        return RedirectToAction("AdventureAlreadyReserved", "Home");
                    }
                    _context.Add(adventureReservation);
                    _context.SaveChanges();
                }
                Debug.WriteLine("sacuvao ssam rezervaciju vikendice");


                //   _context.AdventureReservation.Add(adventureReservation);
                //    await _context.SaveChangesAsync();
                await _emailSender.SendEmailAsync(user.Email.ToString(), "Uspesno ste rezervisali avanturu", $"Uspesno ste rezervisali avanturu '{adventure.Name}' .");

                    InstructorNotAvailablePeriod instructorNotAvailablePeriod = new InstructorNotAvailablePeriod();
                    instructorNotAvailablePeriod.Id = Guid.NewGuid();
                    instructorNotAvailablePeriod.InstructorId = adventure.InstructorId;
                    instructorNotAvailablePeriod.StartTime = adventureRealisation.StartDate;
                    instructorNotAvailablePeriod.EndTime = adventureRealisation.StartDate.AddHours(adventureRealisation.Duration);
                    _context.InstructorNotAvailablePeriod.Add(instructorNotAvailablePeriod);
                    await _context.SaveChangesAsync();
                Debug.WriteLine("sacuvao ssam period nedostupnosti instruktora");

                return RedirectToAction("Index", "Instructors");

            }

            return View(adventureReservation);
        }

    }

}
