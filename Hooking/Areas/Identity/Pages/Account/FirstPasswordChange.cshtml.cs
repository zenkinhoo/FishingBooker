using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Hooking.Areas.Identity.Pages.Account
{
    public class FirstPasswordChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<FirstPasswordChangeModel> _logger;
        private readonly ApplicationDbContext _context;

        public FirstPasswordChangeModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<FirstPasswordChangeModel> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public FirstLoginAdmins FirstLoginAdmin;

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nova lozinka")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrdite lozinku")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            
        }

        public async Task<IActionResult> OnGet(FirstLoginAdmins firstLoginAdmin)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.RemovePasswordAsync(user);
            var changePasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Admin " + user.NormalizedUserName + " set the password successfully.");
            StatusMessage = "Uspešna promena lozinke.";
            var firstLoginAdmin = _context.FirstLoginAdmins.FirstOrDefault(o => o.AdminId == user.Id);
            _context.FirstLoginAdmins.Remove(firstLoginAdmin ?? throw new NullReferenceException());
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
