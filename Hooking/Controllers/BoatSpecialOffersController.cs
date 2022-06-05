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
using System.Text.Encodings.Web;

namespace Hooking.Controllers
{
    public class BoatSpecialOffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public BoatSpecialOffersController(ApplicationDbContext context,
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

        // GET: BoatSpecialOffers
        public async Task<IActionResult> Index(String id)
        {
            Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(id)).FirstOrDefault();

            ViewData["Boat"] = bt;
            return View(await _context.BoatSpecialOffer.Where(m => m.BoatId == id).ToListAsync());
        }

        // GET: BoatSpecialOffers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOffer = await _context.BoatSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            Guid boatId = Guid.Parse(boatSpecialOffer.BoatId);
            Boat boat = _context.Boat.Where(m => m.Id == boatId).FirstOrDefault();
            if (boatSpecialOffer == null)
            {
                return NotFound();
            }
            ViewData["Boat"] = boat;
            return View(boatSpecialOffer);
        }

        // GET: BoatSpecialOffers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoatSpecialOffers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("BoatSpecialOffers/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("StartDate,EndDate,ValidFrom,ValidTo,Price,MaxPersonCount,Description,Id,RowVersion")] BoatSpecialOffer boatSpecialOffer)
        {
            if (ModelState.IsValid)
            {
                boatSpecialOffer.Id = Guid.NewGuid();
                boatSpecialOffer.BoatId = id.ToString();
                boatSpecialOffer.IsReserved = false;
                boatSpecialOffer.StartDate = boatSpecialOffer.StartDate.Date;
                boatSpecialOffer.EndDate = boatSpecialOffer.EndDate.Date;
                boatSpecialOffer.ValidFrom = boatSpecialOffer.ValidFrom.Date;
                boatSpecialOffer.ValidTo = boatSpecialOffer.ValidTo.Date;
                if(await IsPossible(boatSpecialOffer))
                {
                    await CreateSpecialOffer(id, boatSpecialOffer);
                } else
                {
                    return RedirectToAction("ConcurrencyActionError", "Home");
                }
                return RedirectToPage("/Account/Manage/BoatSpecialOffers", new { area = "Identity" });
            }
            return View(boatSpecialOffer);
        }
        public async Task<bool> CreateSpecialOffer(Guid id, BoatSpecialOffer boatSpecialOffer)
        {
            if(await IsPossible(boatSpecialOffer))
            {
                _context.Add(boatSpecialOffer);
                await _context.SaveChangesAsync();
                string boatId = id.ToString();
                List<BoatFavorites> boatFavorites = _context.BoatFavorites.Where(m => m.BoatId == boatId).ToList();
                foreach (var subscribe in boatFavorites)
                {
                    UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == subscribe.UserDetailsId).FirstOrDefault<UserDetails>();
                    var user = await _context.Users.FindAsync(userDetails.IdentityUserId);
                    var callbackUrl = Url.Action("Details", "BoatSpecialOffers", new { id = boatSpecialOffer.Id });

                    await _emailSender.SendEmailAsync(user.Email, "Obaveštenje o specijalnoj akciji",
                               $"Poštovani,<br><br> upravo je objavljena specijalna akcija za brod na koju ste pretplaćeni! Za više detalja kliknite na sledeći link <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ovaj link</a>.");

                }
                return true;
            }
            return false;
        }
        public async Task<bool> IsPossible(BoatSpecialOffer boatSpecialOffer)
        {

            List<BoatReservation> boatReservations = await _context.BoatReservation.Where(m => m.BoatId == boatSpecialOffer.BoatId).ToListAsync();
            foreach (BoatReservation boatReservationTemp in boatReservations)
            {
                if (IsOverlapping(boatSpecialOffer.StartDate, boatSpecialOffer.EndDate, boatReservationTemp.StartDate, boatReservationTemp.EndDate))
                {
                    return false;
                }
            }
            List<BoatSpecialOffer> boatSpecialOffers = await _context.BoatSpecialOffer.Where(m => m.BoatId == boatSpecialOffer.BoatId).ToListAsync();
            foreach (BoatSpecialOffer boatSpecialOfferTemp in boatSpecialOffers)
            {
                if (IsOverlapping(boatSpecialOffer.StartDate, boatSpecialOffer.EndDate, boatSpecialOfferTemp.StartDate, boatSpecialOfferTemp.EndDate))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.BoatReservation.Local)
            {
                if (reservation.BoatId == boatSpecialOffer.BoatId)
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
        // GET: BoatSpecialOffers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOffer = await _context.BoatSpecialOffer.FindAsync(id);
            if (boatSpecialOffer == null)
            {
                return NotFound();
            }
            return View(boatSpecialOffer);
        }

        // POST: BoatSpecialOffers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("StartDate,EndDate,ValidFrom,ValidTo,Price,MaxPersonCount,Description,IsReserved,Id,RowVersion")] BoatSpecialOffer boatSpecialOffer)
        {
            if (id != boatSpecialOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var boatSpecialOfferTemp = _context.BoatSpecialOffer.Where(m => m.Id == id).FirstOrDefault();
                    if(!boatSpecialOfferTemp.IsReserved)
                    {
                        return RedirectToPage("/Account/Manage/BoatSpecialOffers", new { area = "Identity" });
                    }
                    boatSpecialOfferTemp.Id = id;
                    boatSpecialOfferTemp.StartDate = boatSpecialOffer.StartDate.Date;
                    boatSpecialOfferTemp.EndDate = boatSpecialOffer.EndDate.Date;
                    boatSpecialOfferTemp.ValidFrom = boatSpecialOffer.ValidFrom.Date;
                    boatSpecialOfferTemp.ValidTo = boatSpecialOffer.ValidTo.Date;
                    boatSpecialOfferTemp.Price = boatSpecialOffer.Price;
                    boatSpecialOfferTemp.MaxPersonCount = boatSpecialOffer.MaxPersonCount;
                    boatSpecialOfferTemp.Description = boatSpecialOffer.Description;
                    boatSpecialOfferTemp.IsReserved = boatSpecialOffer.IsReserved;
                    _context.Update(boatSpecialOfferTemp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatSpecialOfferExists(boatSpecialOffer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToPage("/Account/Manage/BoatSpecialOffers", new { area = "Identity" });
            }
            return View(boatSpecialOffer);
        }

        // GET: BoatSpecialOffers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatSpecialOffer = await _context.BoatSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatSpecialOffer == null)
            {
                return NotFound();
            }

            return View(boatSpecialOffer);
        }

        // POST: BoatSpecialOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boatSpecialOffer = await _context.BoatSpecialOffer.FindAsync(id);
            _context.BoatSpecialOffer.Remove(boatSpecialOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatSpecialOfferExists(Guid id)
        {
            return _context.BoatSpecialOffer.Any(e => e.Id == id);
        }
    }
}
