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
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Hooking.Controllers
{
    public class CottageSpecialOffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        public Cottage cottage;

        public CottageSpecialOffersController(ApplicationDbContext context,
                                              UserManager<IdentityUser> userManager,
                                              IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            using (StreamReader reader = new StreamReader("./Data/emailCredentials.json"))
            {
                string json = reader.ReadToEnd();
                _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
            }
        }

        // GET: CottageSpecialOffers
        public async Task<IActionResult> Index(String id)
        {
            Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(id)).FirstOrDefault();
            ViewData["Cottage"] = ctg;
            return View(await _context.CottageSpecialOffer.Where(m=>m.CottageId==id).ToListAsync());
        }

        // GET: CottageSpecialOffers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOffer = await _context.CottageSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            Guid cottageId = Guid.Parse(cottageSpecialOffer.CottageId);
            cottage = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault<Cottage>();
            
            if (cottageSpecialOffer == null)
            {
                return NotFound();
            }
            ViewData["Cottage"] = cottage; 
            return View(cottageSpecialOffer);
        }

        // GET: CottageSpecialOffers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CottageSpecialOffers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/CottageSpecialOffers/Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("StartDate,EndDate,ValidFrom,ValidTo,Price,MaxPersonCount,Description,Id,RowVersion")] CottageSpecialOffer cottageSpecialOffer)
        {
            if (ModelState.IsValid)
            {
                cottageSpecialOffer.Id = Guid.NewGuid();
                cottageSpecialOffer.CottageId = id.ToString();
                cottageSpecialOffer.StartDate = cottageSpecialOffer.StartDate.Date;
                cottageSpecialOffer.EndDate = cottageSpecialOffer.EndDate.Date;
                cottageSpecialOffer.ValidFrom = cottageSpecialOffer.ValidFrom.Date;
                cottageSpecialOffer.ValidTo = cottageSpecialOffer.ValidTo.Date;
                cottageSpecialOffer.IsReserved = false;
                if(await IsPossible(cottageSpecialOffer))
                {
                    await CreateSpecialOffer(id, cottageSpecialOffer);
                } else
                {
                    return RedirectToAction("ConcurrencyActionError", "Home");
                }
                return RedirectToPage("/Account/Manage/MySpecialOffers", new { area = "Identity"});
            }
            return View(cottageSpecialOffer);
        }
        public async Task<bool> CreateSpecialOffer(Guid id, CottageSpecialOffer cottageSpecialOffer)
        {
            if(await IsPossible(cottageSpecialOffer))
            {
                _context.Add(cottageSpecialOffer);
                await _context.SaveChangesAsync();
                Cottage cottage = _context.Cottage.Where(m => m.Id == id).FirstOrDefault();
                var cId = id.ToString();
                List<CottageFavorites> cottageFavorites = new List<CottageFavorites>();
                cottageFavorites = _context.CottageFavorites.Where(m => m.CottageId == cId).ToList();
                foreach (var subscribe in cottageFavorites)
                {
                    UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == subscribe.UserDetailsId).FirstOrDefault<UserDetails>();
                    var user = await _context.Users.FindAsync(userDetails.IdentityUserId);
                    var callbackUrl = Url.Action("Details", "CottageSpecialOffers", new { id = cottageSpecialOffer.Id });

                    await _emailSender.SendEmailAsync(user.Email, "Obaveštenje o specijalnoj akciji",
                               $"Poštovani,<br><br> upravo je objavljena specijalna akcija za vikendicu na koju ste pretplaćeni! Za više detalja kliknite na sledeći link <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ovaj link</a>.");

                }
                return true;
            }
            return false;
        }
        public async Task<bool> IsPossible(CottageSpecialOffer cottageSpecialOffer)
        {
           
            List<CottageReservation> cottageReservations = await _context.CottageReservation.Where(m => m.CottageId == cottageSpecialOffer.CottageId).ToListAsync();
            foreach (CottageReservation cottageReservationTemp in cottageReservations)
            {
                if (IsOverlapping(cottageSpecialOffer.StartDate, cottageSpecialOffer.EndDate, cottageReservationTemp.StartDate, cottageReservationTemp.EndDate))
                {
                    return false;
                }
            }
            List<CottageSpecialOffer> cottageSpecialOffers = await _context.CottageSpecialOffer.Where(m => m.CottageId == cottageSpecialOffer.CottageId).ToListAsync();
            foreach (CottageSpecialOffer cottageSpecialOfferTemp in cottageSpecialOffers)
            {
                if (IsOverlapping(cottageSpecialOffer.StartDate, cottageSpecialOffer.EndDate, cottageSpecialOfferTemp.StartDate, cottageSpecialOfferTemp.EndDate))
                {
                    return false;
                }
            }
            foreach (var reservation in _context.CottageReservation.Local)
            {
                if (reservation.CottageId == cottageSpecialOffer.CottageId)
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
        private async void SendNotificationToSubscribers(Guid cottageId, Guid specialOfferId)
        {
            Cottage cottage = _context.Cottage.Where(m => m.Id == cottageId).FirstOrDefault();
            var cId = cottageId.ToString();
            List<CottageFavorites> cottageFavorites = new List<CottageFavorites>();
            cottageFavorites = _context.CottageFavorites.Where(m => m.CottageId == cId).ToList();
            foreach(var subscribe in cottageFavorites)
            {
                UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == subscribe.UserDetailsId).FirstOrDefault<UserDetails>();
                var user = await _context.Users.FindAsync(userDetails.IdentityUserId);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                           "/CottageSpecialOffers/Details",
                           pageHandler: null,
                           values: new { id = specialOfferId },
                           protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Obaveštenje o specijalnoj akciji",
                           $"Poštovani,<br><br> upravo je objavljena specijalna akcija za vikendicu na koju ste pretplaćeni! Za više detalja kliknite na sledeći link <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ovaj link</a>.");

            }
        }
        // GET: CottageSpecialOffers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOffer = await _context.CottageSpecialOffer.FindAsync(id);
            if (cottageSpecialOffer == null)
            {
                return NotFound();
            }
            return View(cottageSpecialOffer);
        }

        // POST: CottageSpecialOffers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CottageId,StartDate,EndDate,ValidFrom,ValidTo,Price,MaxPersonCount,Description,IsReserved,Id,RowVersion")] CottageSpecialOffer cottageSpecialOffer)
        {
            if (id != cottageSpecialOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cottageSpecialOfferTmp = await _context.CottageSpecialOffer.FindAsync(id);
                    if(!cottageSpecialOfferTmp.IsReserved)
                    {
                        return RedirectToPage("/Account/Manage/MySpecialOffers", new { area = "Identity" });
                    }
                    cottageSpecialOfferTmp.Id = id;
                    cottageSpecialOfferTmp.CottageId = cottageSpecialOffer.CottageId;
                    cottageSpecialOfferTmp.StartDate = cottageSpecialOffer.StartDate.Date;
                    cottageSpecialOfferTmp.EndDate = cottageSpecialOffer.EndDate.Date;
                    cottageSpecialOfferTmp.ValidFrom = cottageSpecialOffer.ValidFrom.Date;
                    cottageSpecialOfferTmp.ValidTo = cottageSpecialOffer.ValidTo.Date;
                    cottageSpecialOfferTmp.Price = cottageSpecialOffer.Price;
                    cottageSpecialOfferTmp.MaxPersonCount = cottageSpecialOffer.MaxPersonCount;
                    cottageSpecialOfferTmp.Description = cottageSpecialOffer.Description;
                    cottageSpecialOfferTmp.IsReserved = cottageSpecialOffer.IsReserved;
                    _context.Update(cottageSpecialOfferTmp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CottageSpecialOfferExists(cottageSpecialOffer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToPage("/Account/Manage/MySpecialOffers", new { area = "Identity" });
            }
            return View(cottageSpecialOffer);
        }

        // GET: CottageSpecialOffers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cottageSpecialOffer = await _context.CottageSpecialOffer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cottageSpecialOffer == null)
            {
                return NotFound();
            }

            return View(cottageSpecialOffer);
        }

        // POST: CottageSpecialOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cottageSpecialOffer = await _context.CottageSpecialOffer.FindAsync(id);
            _context.CottageSpecialOffer.Remove(cottageSpecialOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CottageSpecialOfferExists(Guid id)
        {
            return _context.CottageSpecialOffer.Any(e => e.Id == id);
        }
    }
}
