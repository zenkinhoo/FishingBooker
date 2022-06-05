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
    public class CottageReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
       
        public CottageReportsModel(ApplicationDbContext context,
                                    UserManager<IdentityUser> userManager
                                    )
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
            var cottageOwner = _context.CottageOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var cottageOwnerId = cottageOwner.Id.ToString();
            List<Cottage> cottages = _context.Cottage.Where(m => m.CottageOwnerId == cottageOwnerId).ToList();
            double totalIncome = 0;
            int totalReservations = 0;
            foreach(Cottage cottage in cottages)
            {
                string cottageId = cottage.Id.ToString();
                List<CottageReservation> cottageReservations = _context.CottageReservation.Where(m => m.CottageId == cottageId).ToList();
                foreach(CottageReservation cottageReservation in cottageReservations)
                {
                    if(cottageReservation.EndDate <= DateTime.Now)
                    {
                        totalIncome += cottageReservation.Price;
                        totalReservations++;
                    }
                }
                List<CottageSpecialOffer> cottageSpecialOffers = _context.CottageSpecialOffer.Where(m => m.CottageId == cottageId).ToList();
                foreach(CottageSpecialOffer cottageSpecialOffer in cottageSpecialOffers)
                {
                    if(cottageSpecialOffer.EndDate <= DateTime.Now && cottageSpecialOffer.IsReserved == true)
                    {
                        totalIncome += cottageSpecialOffer.Price;
                        totalReservations++;
                    }
                }
            }
            ViewData["TotalIncome"] = totalIncome;
            ViewData["TotalReservations"] = totalReservations;
            return Page();
        }
    }
}
