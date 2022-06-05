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
    public class MySpecialOffersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public List<CottageSpecialOffer> cottageSpecialOffers { get; set; }
        public List<Cottage> cottages = new List<Cottage>();
        [BindProperty]
        public List<string> cottageNames { get; set; }
        public MySpecialOffersModel(UserManager<IdentityUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    SignInManager<IdentityUser> signInManager,
                                    IEmailSender emailSender,
                                    ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var cottageOwner = _context.CottageOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var cottageOwnerId = cottageOwner.Id.ToString();
            List<Cottage> myCottages = new List<Cottage>();
            myCottages =   _context.Cottage.Where(m => m.CottageOwnerId == cottageOwnerId).ToList();
            cottageSpecialOffers = new List<CottageSpecialOffer>();
            cottageNames = new List<string>();
            foreach(var cottage in myCottages)
            {
                var cottageId = cottage.Id.ToString();
                List<CottageSpecialOffer> specialOffers = _context.CottageSpecialOffer.Where(m => m.CottageId == cottageId).ToList<CottageSpecialOffer>();
                foreach(var specialOffer in specialOffers)
                {
                      cottageSpecialOffers.Add(specialOffer);
                        Guid cottageGuid = Guid.Parse(specialOffer.CottageId);
                        var cottageSpec = _context.Cottage.Where(m => m.Id == cottageGuid).FirstOrDefault<Cottage>();
                        cottages.Add(cottageSpec);
                        cottageNames.Add(cottage.Name);
                    
                        
                    
                }
            }
            ViewData["CottageNames"] = cottageNames;
            return Page();
        }
    }
}
