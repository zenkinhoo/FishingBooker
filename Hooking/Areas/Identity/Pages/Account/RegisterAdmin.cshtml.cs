using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hooking.Areas.Identity.Pages.Account
{

    [AllowAnonymous]
    public partial class RegisterAdminModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterAdminModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterAdminModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterAdminModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
            
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required(ErrorMessage = "Polje 'E-mail adresa' je obavezno.")]
            [EmailAddress(ErrorMessage = "E-mail adresa nije u validnom formatu.")]
            [Display(Name = "E-mail adresa")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Polje 'Lozinka' je obavezno.")]
            [StringLength(100, ErrorMessage = "{0} mora biti dugačka bar {2} i najviše {1} karaktera.", MinimumLength = 6)] 
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrdite lozinku")]
            [Compare("Password", ErrorMessage = "Unete lozinke se ne poklapaju.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Polje 'Ime' je obavezno.")]
            [Display(Name = "Ime")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Polje 'Prezime' je obavezno.")]
            [Display(Name = "Prezime")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Polje 'Grad i država' je obavezno.")]
            [Display(Name = "Grad i država")]
            public string Location { get; set; }

            [Required(ErrorMessage = "Odaberite jedan tip registracije.")]
            [Display(Name = "Tip registracije")]
            public RegistrationType Type { get; set; }

            [Required(ErrorMessage = "Polje 'Obrazloženje' je obavezno.")]
            [StringLength(100, MinimumLength = 10, ErrorMessage = "Obrazloženje mora sadržati bar 10, a najviše 100 karaktera.")]
            [Display(Name = "Obrazloženje")]
            public string Description { get; set; }

        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //treba da registrujes admina (iako bi ovo trebalo da je sad vec otkucano, proveri)
            if(ModelState.IsValid)
            {
                var user = GetIdentityUserFromInput();
                var userDetails = GetUserDetailsFromInput();
                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    userDetails.IdentityUserId = user.Id;
                    var resultUserDetails = _context.Add(userDetails);

                    if (resultUserDetails != null)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        _logger.LogInformation(resultUserDetails.GetType().ToString());
                        await _context.SaveChangesAsync();

                        var roleName = "Admin";
                        await _userManager.AddToRoleAsync(user, roleName);
                        await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user)); // confirm email by default

                        _context.Add(new FirstLoginAdmins()
                        {
                            AdminId = user.Id
                        });

                        await _context.SaveChangesAsync();

                        StatusMessage = "Uspešno kreiranje administratora.";
                        return RedirectToPage();
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        private UserDetails GetUserDetailsFromInput()
        {
            UserDetails userDetails = new UserDetails
            {
                FirstName = Input.Name,
                LastName = Input.LastName,
                City = Input.Location.Split(",")[0],
                Country = Input.Location.Split(",")[1],
                Approved = Input.Type == RegistrationType.REGULAR
            };

            return userDetails;
        }

        private IdentityUser GetIdentityUserFromInput()
        {
            return new IdentityUser { UserName = Input.Email, Email = Input.Email};
        }
    }
}
