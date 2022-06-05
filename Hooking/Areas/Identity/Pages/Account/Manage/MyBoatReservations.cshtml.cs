using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class MyBoatReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<BoatReservation> myBoatReservations { get; set; }

        public MyBoatReservationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.IdentityUserId.ToString();
            System.Diagnostics.Debug.WriteLine("identity ud id ulogovanog usera " + userDetailsId);
        //    var boatOwner = _context.BoatOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
         //   var boatOwnerId = boatOwner.Id.ToString();
            myBoatReservations = await _context.BoatReservation.Where(m => m.UserDetailsId == userDetailsId).ToListAsync();
            List<Boat> myBoats = new List<Boat>();
            foreach (var myBoatReservation in myBoatReservations)
            {
                System.Diagnostics.Debug.WriteLine("prosao " );

                Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(myBoatReservation.BoatId)).FirstOrDefault<Boat>();
                myBoats.Add(bt);

            }
            ViewData["Boat"] = myBoats;
            return Page();
        }
    }
}
