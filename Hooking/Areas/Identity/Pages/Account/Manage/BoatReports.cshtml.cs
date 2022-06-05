using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class BoatReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public BoatReportsModel(ApplicationDbContext context,
                                UserManager<IdentityUser> userManager)
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
            List<Boat> boats = _context.Boat.Where(m => m.BoatOwnerId == boatOwnerId).ToList();
            double totalIncome = 0;
            int totalReservations = 0;
            foreach(Boat boat in boats)
            {
                string boatId = boat.Id.ToString();
                List<BoatReservation> boatReservations = _context.BoatReservation.Where(m => m.BoatId == boatId).ToList();
                foreach(BoatReservation boatReservation in boatReservations)
                {
                    if(boatReservation.EndDate <= DateTime.Now)
                    {
                        totalIncome += boatReservation.Price;
                        totalReservations++;
                    }
                }
                List<BoatSpecialOffer> boatSpecialOffers = _context.BoatSpecialOffer.Where(m => m.BoatId == boatId).ToList();
                foreach(BoatSpecialOffer boatSpecialOffer in boatSpecialOffers)
                {
                    if(boatSpecialOffer.EndDate <= DateTime.Now && boatSpecialOffer.IsReserved == true)
                    {
                        totalIncome += boatSpecialOffer.Price;
                        totalReservations++;
                    }
                }
            }
            ViewData["TotalIncome"] = Math.Round(totalIncome,2);
            ViewData["TotalReservations"] = totalReservations;
            return Page();
        }
    }
}
