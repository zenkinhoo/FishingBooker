using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hooking.Models;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class CottagesReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public List<CottageReservation> cottageReservations { get; set; }
        [BindProperty]
        public List<string> cottageNames { get; set; }
        public CottagesReservationsModel(ApplicationDbContext context,
                                          UserManager<IdentityUser> userManager,
                                          RoleManager<IdentityRole> roleManager,
                                          SignInManager<IdentityUser> signInManager,
                                          IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var cottageOwner = _context.CottageOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var cottageOwnerId = cottageOwner.Id.ToString();
            List<Cottage> myCottages = _context.Cottage.Where(m => m.CottageOwnerId == cottageOwnerId).ToList();
            cottageReservations = new List<CottageReservation>();
            cottageNames = new List<string>();
            foreach (var cottage in myCottages)
            {
                var cottageId = cottage.Id.ToString();
                List<CottageReservation> myCottageReservations = new List<CottageReservation>();
                myCottageReservations = _context.CottageReservation.Where(m => m.CottageId == cottageId).ToList();
                if (myCottageReservations.Count != 0)
                {
                    foreach (CottageReservation cottageReservation in myCottageReservations)
                    {
                        if (cottageReservation.StartDate >= DateTime.Now)
                        {
                            cottageReservations.Add(cottageReservation);
                            cottageNames.Add(cottage.Name);
                        }
                    }
                }

            }
            ViewData["CottageNames"] = cottageNames;
            return Page();
        }
    }
}

