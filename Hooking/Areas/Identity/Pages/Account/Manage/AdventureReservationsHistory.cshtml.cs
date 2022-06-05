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
    public class AdventureReservationsHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public List<AdventureReservation> myAdventureReservations { get; set; }

        public List<AdventureRealisation> adRealisations = new List<AdventureRealisation>();
        public string StartDateSort { get; set; }
        public string PriceSort { get; set; }

        public string DurationSort { get; set; }

        public AdventureReservationsHistoryModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string sortOrder = "")
        {



            IQueryable<AdventureRealisation> reservationToSort = from s in _context.AdventureRealisation
                                                            select s;
            switch (sortOrder)
            {
                case "StartDate":
                    reservationToSort = reservationToSort.OrderBy(s => s.StartDate);
                    break;

                case "Price":
                    reservationToSort = reservationToSort.OrderBy(s => s.Price);
                    break;

                case "Duration":
                    reservationToSort = reservationToSort.OrderBy(s => s.Duration);
                    break;


            }
            adRealisations = await reservationToSort.AsNoTracking().ToListAsync();


            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();

            //  System.Diagnostics.Debug.WriteLine(user.Id.ToString());
            myAdventureReservations = await _context.AdventureReservation.Where(m=>m.UserDetailsId == userDetails.IdentityUserId.ToString()).ToListAsync();
            List<AdventureRealisation> myAdventureRealisations = adRealisations;
         /*   foreach (var myAdventureReservation in myAdventureReservations)
            {
               AdventureRealisation ar =  _context.AdventureRealisation.Where(m => m.Id == Guid.Parse(myAdventureReservation.AdventureRealisationId)).FirstOrDefault<AdventureRealisation>();
               myAdventureRealisations.Add(ar);
            }*/
            List<AdventureRealisation> finalAdventureRealisations = new List<AdventureRealisation>();

            foreach (var myAdventureRealisation in myAdventureRealisations)
            {
                foreach(var adRealisation in adRealisations)
                {
                    if(myAdventureRealisation.StartDate==adRealisation.StartDate)
                    {
                        if(!finalAdventureRealisations.Contains(myAdventureRealisation))
                        {
                            finalAdventureRealisations.Add(myAdventureRealisation);
                        }
                        
                    }
                }
            }


            List<Adventure> myAdventures = new List<Adventure>();
            foreach (var myAdventureRealisation in finalAdventureRealisations)
            {
                Adventure adv = _context.Adventure.Where(m => m.Id == Guid.Parse(myAdventureRealisation.AdventureId)).FirstOrDefault<Adventure>();
                myAdventures.Add(adv);
            }
            List<Instructor> instructors = new List<Instructor>();
            foreach (var myAdventure in myAdventures)
            {
                Instructor ins = _context.Instructor.Where(m => m.Id == Guid.Parse(myAdventure.InstructorId)).FirstOrDefault<Instructor>();
                instructors.Add(ins);
            }
            List<UserDetails> userInstructors = new List<UserDetails>();
            foreach (var instructor in instructors)
            {
                UserDetails userInstructor = _context.UserDetails.Where(m => m.Id == Guid.Parse(instructor.UserDetailsId)).FirstOrDefault<UserDetails>();
                userInstructors.Add(userInstructor);
            }

            ViewData["AdventureRealisations"] = myAdventureRealisations;
            ViewData["AdventureReservations"] = myAdventureReservations;
            ViewData["Adventure"] = myAdventures;
            ViewData["Instructor"] = instructors;
            ViewData["UserInstructor"] = userInstructors;





            return Page();
        }
    }
}
