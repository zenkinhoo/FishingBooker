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
    public class UserDeleteRequestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public string Description { get; set; }
        public UserDeleteRequestModel(ApplicationDbContext context,
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
        public async Task<IActionResult> OnCreate()
        {
            var user = await _userManager.GetUserAsync(User);
            UserDeleteRequest userDeleteRequest = new UserDeleteRequest {Id = Guid.NewGuid(), IsApproved = false};
            UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == user.Id);

            if (userDetails == null)
            {
                return NotFound();
            }

            userDeleteRequest.UserDetailsId = userDetails.Id.ToString();
            IList<string> rolenames = await _signInManager.UserManager.GetRolesAsync(user);
            switch(rolenames[0])
            {
                case "Vlasnik vikendice":
                    userDeleteRequest.Type = DeletionType.COTTAGEOWNER;
                    break;
                case "Korisnik":
                    userDeleteRequest.Type = DeletionType.USER;
                    break;
                case "Instruktor":
                    userDeleteRequest.Type = DeletionType.INSTRUCTOR;
                    break;
                case "Vlasnik broda":
                    userDeleteRequest.Type = DeletionType.BOATOWNER;
                    break;
                case "Admin":
                    userDeleteRequest.Type = DeletionType.ADMIN;
                    break;
            }
            userDeleteRequest.Description = Description;
            _context.Add(userDeleteRequest);
            StatusMessage = "Zahtev za brisanje profila uspešno podnet.";
            return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
        }
       
    }
}
