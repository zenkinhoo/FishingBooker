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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Hooking.Controllers
{
    public class BoatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BlobUtility _utility;
        public BoatsController(ApplicationDbContext context,
                                UserManager<IdentityUser> userManager, 
                                RoleManager<IdentityRole> roleManager
                                )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _utility = new BlobUtility();
        }

        // GET: Boats
        public async Task<IActionResult> Index(string searchString = "",string filter="",string sortOrder="")
        {
            List<Boat> boats = await _context.Boat.ToListAsync();

            var bts = from b in _context.Boat
                      select b;
            switch (sortOrder)
            {
                case "Name":
                    bts = bts.OrderBy(b => b.Name);
                    break;
                case "Address":
                    bts = bts.OrderBy(b => b.Address);
                    break;
                case "City":
                    bts = bts.OrderBy(b => b.City);
                    break;
                case "Country":
                    bts = bts.OrderBy(b => b.Country);
                    break;
                case "AverageGrade":
                    bts = bts.OrderByDescending(b => b.AverageGrade);
                    break;

            }
            switch (filter)
            {
                case "Name":
                    bts = bts.Where(s => s.Name.Contains(searchString));
                    break;
                case "City":
                    bts = bts.Where(s => s.City.Contains(searchString));
                    break;
                case "Country":
                    bts = bts.Where(s => s.Country.Contains(searchString));
                    break;
            }
            return View(bts.ToList());
        }
        //GET : BoatOwner/Details/5/ShowMyBoats
        public async Task<IActionResult> ShowMyBoats()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var boatOwnerId = boatOwner.Id.ToString();
            var boats = await _context.Boat.Where(m => m.BoatOwnerId == boatOwnerId).ToListAsync();
            return View(boats);
        }

        // GET: Boats/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .FirstOrDefaultAsync(m => m.Id == id);
            var boatId = boat.Id.ToString();
            var boatOwnerId = Guid.Parse(boat.BoatOwnerId);
            var boatOwner = _context.BoatOwner.Where(m => m.Id == boatOwnerId).FirstOrDefault();
            var boatOwnerUserId = Guid.Parse(boatOwner.UserDetailsId);
            var boatOwnerUser = _context.UserDetails.Where(m => m.Id == boatOwnerUserId).FirstOrDefault();
            var fullAddress = boat.Address + "," + boat.City + "," + boat.Country;
            Guid boatCancelationPolicyId = Guid.Parse(boat.CancelationPolicyId);
            CancelationPolicy cancelationPolicy = _context.CancelationPolicy.Where(m => m.Id == boatCancelationPolicyId).FirstOrDefault();
            BoatFishingEquipment boatFishingEquipment = _context.BoatFishingEquipment.Where(m => m.BoatId == boatId).FirstOrDefault();
            Guid fishingEquipmentId = Guid.Parse(boatFishingEquipment.FishingEquipment);
            FishingEquipment fishingEquipment = _context.FishingEquipment.Where(m => m.Id == fishingEquipmentId).FirstOrDefault();
            BoatAmenities boatAmenities = _context.BoatAmenities.Where(m => m.BoatId == boatId).FirstOrDefault();
            Guid amenitiesId = Guid.Parse(boatAmenities.AmanitiesId);
            Amenities amenities = _context.Amenities.Where(m => m.Id == amenitiesId).FirstOrDefault();
            if (boat == null)
            {
                return NotFound();
            }
            List<BoatImage> boatImages = _context.BoatImage.Where(m => m.BoatId == boatId).ToList<BoatImage>();
            ViewData["BoatOwner"] = boatOwnerUser;
            ViewData["FullAddress"] = fullAddress;
            ViewData["CancelationPolicy"] = cancelationPolicy;
            ViewData["FishingEquipment"] = fishingEquipment;
            ViewData["BoatImages"] = boatImages;
            ViewData["Amenities"] = amenities;
            return View(boat);
        }
        [HttpGet("/Boats/MyBoatDetails/{id}")]
        public async Task<IActionResult> MyBoatDetails(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .FirstOrDefaultAsync(m => m.Id == id);
            var boatId = boat.Id.ToString();
            var boatOwnerId = Guid.Parse(boat.BoatOwnerId);
            var boatOwner = _context.BoatOwner.Where(m => m.Id == boatOwnerId).FirstOrDefault();
            var boatOwnerUserId = Guid.Parse(boatOwner.UserDetailsId);
            var boatOwnerUser = _context.UserDetails.Where(m => m.Id == boatOwnerUserId).FirstOrDefault();
            var fullAddress = boat.Address + "," + boat.City + "," + boat.Country;
            Guid boatCancelationPolicyId = Guid.Parse(boat.CancelationPolicyId);
            CancelationPolicy cancelationPolicy = _context.CancelationPolicy.Where(m => m.Id == boatCancelationPolicyId).FirstOrDefault();
            BoatFishingEquipment boatFishingEquipment = _context.BoatFishingEquipment.Where(m => m.BoatId == boatId).FirstOrDefault();
            Guid fishingEquipmentId = Guid.Parse(boatFishingEquipment.FishingEquipment);
            FishingEquipment fishingEquipment = _context.FishingEquipment.Where(m => m.Id == fishingEquipmentId).FirstOrDefault();
            BoatAmenities boatAmenities = _context.BoatAmenities.Where(m => m.BoatId == boatId).FirstOrDefault();
            Guid amenitiesId = Guid.Parse(boatAmenities.AmanitiesId);
            Amenities amenities = _context.Amenities.Where(m => m.Id == amenitiesId).FirstOrDefault();
            if (boat == null)
            {
                return NotFound();
            }
            List<BoatImage> boatImages = _context.BoatImage.Where(m => m.BoatId == boatId).ToList<BoatImage>();
            ViewData["BoatOwner"] = boatOwnerUser;
            ViewData["FullAddress"] = fullAddress;
            ViewData["CancelationPolicy"] = cancelationPolicy;
            ViewData["FishingEquipment"] = fishingEquipment;
            ViewData["BoatImages"] = boatImages;
            ViewData["Amenities"] = amenities;
            return View(boat);
        }


        // GET: Boats/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult BoatSpecialOffers()
        {
            return Redirect("/BoatSpecialOffers");
        }
        [HttpGet("/Boats/BoatsForSpecialOffer")]
        public async Task<IActionResult> BoatsForSpecialOffer()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var boatOwnerId = boatOwner.Id.ToString();
            List<Boat> boats = _context.Boat.Where(m => m.BoatOwnerId == boatOwnerId).ToList<Boat>();
            return View(boats);
        }
        [HttpGet("/Boats/BoatsForReservation")]
        public async Task<IActionResult> BoatsForReservation()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var boatOwnerId = boatOwner.Id.ToString();
            List<Boat> boats = _context.Boat.Where(m => m.BoatOwnerId == boatOwnerId).ToList<Boat>();
            return View(boats);
        }
        // POST: Boats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Length,Capacity,EngineNumber,EnginePower,MaxSpeed,Address,City,Country,CancellationPolicyId,Description,AverageGrade,GradeCount,RegularPrice,WeekendPrice,BoatOwnerId,Id,RowVersion")] Boat boat)
        {
            if (ModelState.IsValid)
            {
                boat.Id = Guid.NewGuid();
                var user = await _userManager.GetUserAsync(User);
                var userId = Guid.Parse(user.Id);
                var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
                var userDetailsId = userDetails.Id.ToString();
                var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();

                boat.BoatOwnerId = boatOwner.Id.ToString();
                boat.CancelationPolicyId = "0";
                boat.AverageGrade = 0;
                boat.GradeCount = 0;
                _context.Add(boat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "BoatRules", new { id = boat.Id });
            }
            return View(boat);
        }

        // GET: Boats/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat.FindAsync(id);

            if (boat == null)
            {
                return NotFound();
            }
            return View(boat);
        }

        // POST: Boats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Address,City,Country,Description,RegularPrice,WeekendPrice,Id,RowVersion")] Boat boat)
        {
            if (id != boat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string boatId = id.ToString();
                    List<BoatReservation> futureReservations = new List<BoatReservation>();
                    List<BoatReservation> boatReservations = _context.BoatReservation.Where(m => m.BoatId == boatId).ToList();
                    List<BoatSpecialOffer> reservedSpecialOffers = new List<BoatSpecialOffer>();
                    List<BoatSpecialOffer> boatSpecialOffers = _context.BoatSpecialOffer.Where(m => m.BoatId == boatId).ToList();
                    foreach(BoatSpecialOffer boatSpecialOffer in boatSpecialOffers)
                    {
                        if(boatSpecialOffer.IsReserved && boatSpecialOffer.StartDate >= DateTime.Now)
                        {
                            reservedSpecialOffers.Add(boatSpecialOffer);
                        }

                    }
                    foreach(BoatReservation boatReservation in boatReservations)
                    {
                        if(boatReservation.StartDate >= DateTime.Now)
                        {
                            futureReservations.Add(boatReservation);
                        }
                    }
                    if(futureReservations.Count == 0 && reservedSpecialOffers.Count == 0)
                    {
                        var boatTmp = await _context.Boat.FindAsync(id);
                        boatTmp.Name = boat.Name;
                        boatTmp.Address = boat.Address;
                        boatTmp.City = boat.City;
                        boatTmp.Country = boat.Country;
                        boatTmp.Description = boat.Description;
                        boatTmp.RegularPrice = boat.RegularPrice;
                        boatTmp.WeekendPrice = boat.WeekendPrice;
                        _context.Update(boatTmp);
                        await _context.SaveChangesAsync();
                    }
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatExists(boat.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToPage("/Account/Manage/MyBoats", new { area = "Identity" });
            }
            return View(boat);
        }

        // GET: Boats/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // POST: Boats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boat = await _context.Boat.FindAsync(id);
            string boatId = id.ToString();
            List<BoatReservation> futureReservations = new List<BoatReservation>();
            List<BoatReservation> boatReservations = _context.BoatReservation.Where(m => m.BoatId == boatId).ToList();
            List<BoatSpecialOffer> reservedSpecialOffers = new List<BoatSpecialOffer>();
            List<BoatSpecialOffer> boatSpecialOffers = _context.BoatSpecialOffer.Where(m => m.BoatId == boatId).ToList();
            foreach (BoatSpecialOffer boatSpecialOffer in boatSpecialOffers)
            {
                if (boatSpecialOffer.IsReserved)
                {
                    reservedSpecialOffers.Add(boatSpecialOffer);
                }

            }
            foreach (BoatReservation boatReservation in boatReservations)
            {
                if (boatReservation.StartDate >= DateTime.Now)
                {
                    futureReservations.Add(boatReservation);
                }
            }
            if (futureReservations.Count == 0 && reservedSpecialOffers.Count == 0)
            {
                _context.Boat.Remove(boat);
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("/Boats/UploadImage/{id}")]
        public async Task<ActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file != null)
            {
                string ContainerName = "boat"; //hardcoded container name
                string fileName = Path.GetFileName(file.FileName);
                using (var fileStream = file.OpenReadStream())
                {
                    string UserConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName=hookingstorage;AccountKey=+v8L5XkQZ7Z2CTfdTd03pngWlA4xu02caFJDGUkvGlo/rv8uZnM9CQQYleH3lpb+3Z8sefUOlC0EaoXWIquyDg==;EndpointSuffix=core.windows.net");
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(UserConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(ContainerName.ToLower());
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                    try
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);

                    }
                    catch (Exception e)
                    {
                        var r = e.Message;
                        return null;
                    }

                    if (blockBlob != null)
                    {
                        BoatImage boatImage = new BoatImage();
                        boatImage.Id = Guid.NewGuid();
                        boatImage.BoatId = id.ToString();
                        boatImage.ImageUrl = blockBlob.Uri.ToString();
                        _context.BoatImage.Add(boatImage);
                        await _context.SaveChangesAsync();
                    }

                }
                return RedirectToPage("/Account/Manage/MyBoats", new { area = "Identity" });
            }
            else
            {
                return RedirectToPage("/Account/Manage/MyBoats", new { area = "Identity" });
            }


        }
        private bool BoatExists(Guid id)
        {
            return _context.Boat.Any(e => e.Id == id);
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

        public async Task<IActionResult> BoatsFiltered(DateTime StartDate, DateTime EndDate, double price = 0, string City = "", double AverageGrade = 0, int MaxPersonCount = 0)
        {


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
   


            return View(helpBoats);
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
            if (City != null)
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
        [HttpGet("/Boats/FinishBoatReservation")]
        public async Task<IActionResult> FinishBoatReservation(Guid? id, DateTime StartDate, DateTime EndDate, int PersonCount)
        {
            System.Diagnostics.Debug.WriteLine("finishboatres");

            System.Diagnostics.Debug.WriteLine(PersonCount.ToString());
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .FirstOrDefaultAsync(m => m.Id == id);
            var boatId = boat.Id.ToString();
            var boatOwner = _context.BoatOwner.Where(m => m.Id == Guid.Parse(boat.BoatOwnerId)).FirstOrDefault<BoatOwner>();



            var boatOwnerUser = _context.UserDetails.Where(m => m.Id == Guid.Parse(boatOwner.UserDetailsId)).FirstOrDefault<UserDetails>();

            var fullAddress = boat.Address + "," + boat.City + "," + boat.Country;
            Guid boatCancelationPolicyId = Guid.Parse(boat.CancelationPolicyId);
            CancelationPolicy cancelationPolicy = _context.CancelationPolicy.Where(m => m.Id == boatCancelationPolicyId).FirstOrDefault<CancelationPolicy>();
            BoatFishingEquipment boatFishingEquipment = _context.BoatFishingEquipment.Where(m => m.BoatId == boatId).FirstOrDefault<BoatFishingEquipment>();
            Guid fishingEquipmentId = Guid.Parse(boatFishingEquipment.FishingEquipment);
            FishingEquipment fishingEquipment = _context.FishingEquipment.Where(m => m.Id == fishingEquipmentId).FirstOrDefault<FishingEquipment>();
            BoatAmenities boatAmenities = _context.BoatAmenities.Where(m => m.BoatId == boatId).FirstOrDefault<BoatAmenities>();
            Guid amenitiesId = Guid.Parse(boatAmenities.AmanitiesId);
            Amenities amenities = _context.Amenities.Where(m => m.Id == amenitiesId).FirstOrDefault<Amenities>();
            if (boat == null)
            {
                return NotFound();
            }
            List<BoatImage> boatImages = _context.BoatImage.Where(m => m.BoatId == boatId).ToList<BoatImage>();
            ViewData["BoatOwner"] = boatOwner;
            ViewData["BoatOwnerUser"] = boatOwnerUser;

            ViewData["FullAddress"] = fullAddress;
            ViewData["CancelationPolicy"] = cancelationPolicy;
            ViewData["FishingEquipment"] = fishingEquipment;
            ViewData["BoatImages"] = boatImages;
            ViewData["Amenities"] = amenities;
          
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["PersonCount"] = PersonCount;

            return View(boat);
        }
    }
}
