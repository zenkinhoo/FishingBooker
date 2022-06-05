using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hooking.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class BoatReservationsHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<BoatReservation> myBoatReservations = new List<BoatReservation>();

        public List<BoatReservation> boatReservations { get; set; }
        public List<string> boatNames { get; set; } 
        public BoatReservationsHistoryModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string sortOrder="")
        {

            IQueryable<BoatReservation> reservationToSort = from s in _context.BoatReservation
                                                            select s;
            switch (sortOrder)
            {
                case "StartDate":
                    reservationToSort = reservationToSort.OrderBy(s => s.StartDate);
                    break;
                case "Price":
                    reservationToSort = reservationToSort.OrderBy(s => s.Price);
                    break;
                case "EndDate":
                    reservationToSort = reservationToSort.OrderBy(s => s.EndDate);
                    break;
            }
            boatReservations = await reservationToSort.AsNoTracking().ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();



            foreach (var boatReservation in boatReservations)
            {


                if (boatReservation.UserDetailsId == userDetails.IdentityUserId)
                {
                    myBoatReservations.Add(boatReservation);
                }
            }

            //  myCottageReservations = await _context.CottageReservation.Where(m => m.UserDetailsId == user.Id).ToListAsync();

            List<Boat> myBoats = new List<Boat>();
            foreach (var myBoatReservation in myBoatReservations)
            {

                Boat bt = _context.Boat.Where(m => m.Id == Guid.Parse(myBoatReservation.BoatId)).FirstOrDefault<Boat>();
                myBoats.Add(bt);

            }
            ViewData["Boat"] = myBoats;


            return Page();
        }
    }
}
