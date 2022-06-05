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
    public class BoatReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public List<BoatReservation> boatReservations { get; set; }
        public List<string> boatNames { get; set; }
        public BoatReservationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
            boatReservations = new List<BoatReservation>();
            boatNames = new List<string>();
            foreach (Boat boat in boats)
            {
                string boatId = boat.Id.ToString();
                List<BoatReservation> myBoatReservations = _context.BoatReservation.Where(m => m.BoatId == boatId).ToList<BoatReservation>();
                if (myBoatReservations.Count != 0)
                {
                    foreach (BoatReservation boatReservation in myBoatReservations)
                    {
                        if (boatReservation.StartDate >= DateTime.Now)
                        {
                            boatReservations.Add(boatReservation);
                            boatNames.Add(boat.Name);
                        }
                    }
                }
            }
            ViewData["BoatNames"] = boatNames;
            return Page();
        }
    }
}
