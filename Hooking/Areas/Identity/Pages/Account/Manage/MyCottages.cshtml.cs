using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hooking.Models;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class MyCottagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public List<Cottage> myCottages { get; set; }

        public MyCottagesModel(UserManager<IdentityUser> userManager, 
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
            myCottages = await _context.Cottage.Where(m => m.CottageOwnerId == cottageOwnerId).ToListAsync();
            return Page();
        }
        
    }
}
