using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hooking.Models;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class BoatSpecialOffersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public List<BoatSpecialOffer> boatSpecialOffers { get; set; }
        public List<string> boatNames { get; set; }
        public BoatSpecialOffersModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var boatOwnerId = boatOwner.Id.ToString();
            List<Boat> boats = _context.Boat.Where(m => m.BoatOwnerId == boatOwnerId).ToList<Boat>();
            boatSpecialOffers = new List<BoatSpecialOffer>();
            boatNames = new List<string>();
            foreach (Boat boat in boats)
            {
                var boatId = boat.Id.ToString();
                List<BoatSpecialOffer> boatSpecialOffersTemp = _context.BoatSpecialOffer.Where(m => m.BoatId == boatId).ToList<BoatSpecialOffer>();
                foreach (BoatSpecialOffer boatSpecialOffer in  boatSpecialOffersTemp)
                {
                    
                        boatSpecialOffers.Add(boatSpecialOffer);
                        boatNames.Add(boat.Name);
                    


                }
            }
            ViewData["BoatNames"] = boatNames;
            return Page();
        }
    }
}
