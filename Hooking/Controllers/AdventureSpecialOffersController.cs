using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Hooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;

namespace Hooking.Controllers
{
    public class AdventureSpecialOffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAdventureService _adventureService;
        private readonly IEmailSender _emailSender;
        public AdventureSpecialOffersController(ApplicationDbContext context,
                                                UserManager<IdentityUser> userManager, 
                                                IAdventureService adventureService, 
                                                IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _adventureService = adventureService;
            _emailSender = emailSender;

            using StreamReader reader = new StreamReader("./Data/emailCredentials.json");
            string json = reader.ReadToEnd();
            _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
        }

        // GET: AdventureSpecialOffers
        public async Task<IActionResult> Index(String id)
        {
            var user = await _userManager.GetUserAsync(User);

       /*     Guid userId = Guid.Parse(user.Id);
            System.Diagnostics.Debug.WriteLine(userId);
            UserDetails userDetails = await _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefaultAsync<UserDetails>();
            var userDetailsId = userDetails.Id.ToString();
            Instructor instructor = await _context.Instructor.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefaultAsync<Instructor>();
            string instructorId = instructor.Id.ToString();
            List<Adventure> adventures = await _context.Adventure.Where(m => m.InstructorId == instructorId).ToListAsync<Adventure>();
            List<AdventureSpecialOffer> adventureSpecialOffers = new List<AdventureSpecialOffer>();
            List<string> adventureNames = new List<string>();
            foreach(Adventure adventure in adventures)
            {
                string adventureId = adventure.Id.ToString();
                List<AdventureSpecialOffer> adventureSpecials = await _context.AdventureSpecialOffer.Where(m => m.AdventureId == adventureId).ToListAsync<AdventureSpecialOffer>();
                adventureSpecialOffers.AddRange(adventureSpecials);
                for(int i = 0; i < adventureSpecials.Count; i++)
                    adventureNames.Add(adventure.Name);
            }*/

            Adventure adv = _context.Adventure.Where(m => m.Id == Guid.Parse(id)).FirstOrDefault();

            ViewData["Adventure"] = adv;

          //  ViewData["AdventureNames"] = adventureNames;
            return View(await _context.AdventureSpecialOffer.Where(m => m.AdventureId == id).ToListAsync());
        }
       
        // GET: AdventureSpecialOffers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureSpecialOffer = await _context.AdventureSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureSpecialOffer == null)
            {
                return NotFound();
            }

            return View(adventureSpecialOffer);
        }

        // GET: AdventureSpecialOffers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdventureSpecialOffers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("AdventureSpecialOffers/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id,[Bind("StartDate,ValidFrom,ValidTo,Duration,Price,MaxPersonCount,Description,Id,RowVersion")] AdventureSpecialOffer adventureSpecialOffer)
        {
            if (ModelState.IsValid)
            {
                adventureSpecialOffer.Id = Guid.NewGuid();
                adventureSpecialOffer.AdventureId = id.ToString();
                adventureSpecialOffer.IsReserved = false;
                _context.Add(adventureSpecialOffer);

                if(await IsPossible(adventureSpecialOffer))
                {
                    await CreateSpecialOffer(id, adventureSpecialOffer);
                } else
                {
                    return RedirectToAction("ConcurrencyActionError", "Home");
                }
               

                return RedirectToAction(nameof(Index));
            }
            return View(adventureSpecialOffer);
        }
        public async Task<bool> CreateSpecialOffer(Guid id, AdventureSpecialOffer adventureSpecialOffer)
        {
            if (await IsPossible(adventureSpecialOffer))
            {
                _context.Add(adventureSpecialOffer);
                await _context.SaveChangesAsync();
                string adventureId = id.ToString();
                Adventure adventure = _context.Adventure.Find(id);
                List<AdventureFavorites> adventureFavorites = _context.AdventureFavorites.Where(m => m.AdventureId == adventureId).ToList();
                foreach (var subscribe in adventureFavorites)
                {
                    UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == subscribe.UserDetailsId).FirstOrDefault<UserDetails>();
                    var user = await _context.Users.FindAsync(userDetails.IdentityUserId);
                    var callbackUrl = Url.Action("Details", "BoatSpecialOffers", new { id = adventureSpecialOffer.Id });

                    await _emailSender.SendEmailAsync(user.Email, "Avantura koju pratite je na akciji", $"Avanatura {adventure.Name} je na specijalnoj akciji po ceni od {adventureSpecialOffer.Price} za najviše {adventureSpecialOffer.MaxPersonCount} ljudi, a sa početkom {adventureSpecialOffer.StartDate}!\n\nNe propustite ovu sjajnu priliku!");

                }
                return true;
            }
            return false;
        }
        public async Task<bool> IsPossible(AdventureSpecialOffer adventureSpecialOffer)
        {
            Adventure adventure = _context.Adventure.Find(Guid.Parse(adventureSpecialOffer.AdventureId));
            string adventureId = adventure.Id.ToString();
            List<AdventureRealisation> adventureRealisations = await _context.AdventureRealisation.Where(m => m.AdventureId == adventureId).ToListAsync();
            List<AdventureReservation> adventureReservations = new List<AdventureReservation>();
            foreach(AdventureRealisation adventureRealisationTemp in adventureRealisations)
            {
                string realizationId = adventureRealisationTemp.ToString();
                List<AdventureReservation> adventureReservationsTemp = await _context.AdventureReservation.Where(m => m.AdventureRealisationId == realizationId).ToListAsync();
                adventureReservations.AddRange(adventureReservationsTemp);
            }
            foreach (AdventureReservation adventureReservationTemp in adventureReservations)
            {
                AdventureRealisation adventureRealisation = _context.AdventureRealisation.Find(Guid.Parse(adventureReservationTemp.AdventureRealisationId));
                if (IsOverlapping(adventureSpecialOffer.StartDate, adventureSpecialOffer.StartDate.AddHours(adventureSpecialOffer.Duration), adventureRealisation.StartDate, adventureRealisation.StartDate.AddHours(adventureRealisation.Duration)))
                {
                    return false;
                }
            }
            List<AdventureSpecialOffer> adventureSpecialOffers = await _context.AdventureSpecialOffer.Where(m => m.AdventureId == adventureSpecialOffer.AdventureId).ToListAsync();
            foreach (AdventureSpecialOffer adventureSpecialOfferTemp in adventureSpecialOffers)
            {
                if (IsOverlapping(adventureSpecialOffer.StartDate, adventureSpecialOffer.StartDate.AddHours(adventureSpecialOffer.Duration), adventureSpecialOfferTemp.StartDate, adventureSpecialOfferTemp.StartDate.AddHours(adventureSpecialOfferTemp.Duration)))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.AdventureReservation.Local)
            {
                AdventureRealisation adventureRealisation = _context.AdventureRealisation.Find(Guid.Parse(reservation.AdventureRealisationId));
                if(adventureRealisation.AdventureId == adventureSpecialOffer.AdventureId)
                {
                    return false;
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

        // GET: AdventureSpecialOffers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureSpecialOffer = await _context.AdventureSpecialOffer.FindAsync(id);
            if (adventureSpecialOffer == null)
            {
                return NotFound();
            }
            return View(adventureSpecialOffer);
        }

        // POST: AdventureSpecialOffers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,Description,Id,RowVersion")] AdventureSpecialOffer adventureSpecialOffer)
        {
            if (id != adventureSpecialOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adventureSpecialOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdventureSpecialOfferExists(adventureSpecialOffer.Id))
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
            return View(adventureSpecialOffer);
        }

        // GET: AdventureSpecialOffers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adventureSpecialOffer = await _context.AdventureSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureSpecialOffer == null)
            {
                return NotFound();
            }

            return View(adventureSpecialOffer);
        }

        // POST: AdventureSpecialOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var adventureSpecialOffer = await _context.AdventureSpecialOffer.FindAsync(id);
            _context.AdventureSpecialOffer.Remove(adventureSpecialOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureSpecialOfferExists(Guid id)
        {
            return _context.AdventureSpecialOffer.Any(e => e.Id == id);
        }
    }
}
