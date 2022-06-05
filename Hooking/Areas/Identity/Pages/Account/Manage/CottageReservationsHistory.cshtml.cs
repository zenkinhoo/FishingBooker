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
    public class CottageReservationsHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CottageReservationsHistoryModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        /*   public void OnGet()
           {
           }*/
        public List<CottageReservation> myCottageReservations = new List<CottageReservation>();

        public List<CottageReservation> ctgReservations { get; set; }



        public async Task<IActionResult> OnGetAsync(string sortOrder="")
        {


            IQueryable<CottageReservation> reservationToSort = from s in _context.CottageReservation
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
            ctgReservations = await reservationToSort.AsNoTracking().ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();



            foreach (var ctgReservation in ctgReservations)
            {


                if (ctgReservation.UserDetailsId == userDetails.IdentityUserId)
                {
                    myCottageReservations.Add(ctgReservation);
                }
            }

          //  myCottageReservations = await _context.CottageReservation.Where(m => m.UserDetailsId == user.Id).ToListAsync();
            
            List<Cottage> myCottages = new List<Cottage>();
            foreach(var myCottageReservation in myCottageReservations)
            {
               
                Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(myCottageReservation.CottageId)).FirstOrDefault<Cottage>();
                myCottages.Add(ctg);

            }
            ViewData["Cottage"] = myCottages;
         

            return Page();
        }
    }
}
