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
    public class MyCottageReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<CottageReservation> myCottageReservations { get; set; }

        public MyCottageReservationsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();

            myCottageReservations = await _context.CottageReservation.Where(m => m.UserDetailsId == userDetails.IdentityUserId).ToListAsync();
            List<Cottage> myCottages = new List<Cottage>();

            foreach (var myCottageReservation in myCottageReservations)
            {
                Cottage ctg = _context.Cottage.Where(m => m.Id == Guid.Parse(myCottageReservation.CottageId)).FirstOrDefault<Cottage>();
                myCottages.Add(ctg);

            }
            ViewData["Cottage"] = myCottages;
            return Page();
        }
    }
}
