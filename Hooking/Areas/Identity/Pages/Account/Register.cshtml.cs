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
    public enum RegistrationType
    {
        [Display(Name = "Običan korisnik")]
        REGULAR,
        [Display(Name = "Vlasnik vikendice")]
        COTTAGE_OWNER,
        [Display(Name = "Vlasnik broda")]
        BOAT_OWNER,
        [Display(Name = "Instruktor")]
        INSTRUCTOR
    }

    [AllowAnonymous]
    public partial class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,            
            IEmailSender emailSender, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;

            using (StreamReader reader = new StreamReader("./Data/emailCredentials.json"))
            {
                string json = reader.ReadToEnd();
                _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
            }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }


        private RegistrationRequest CreateRegistrationRequest(UserDetails userDetails)
        {
            RegistrationRequest request = new RegistrationRequest
            {
                UserDetailsId = userDetails.Id.ToString(),
                Type = Input.Type,
                Description = Input.Description
            };

            return request;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            Debug.WriteLine(returnUrl);
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
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

                        var roleName = GetInputType();
                        await _userManager.AddToRoleAsync(user, roleName);
                        await _context.SaveChangesAsync();

                        if (Input.Type != RegistrationType.REGULAR)
                        {
                            RegistrationRequest request = CreateRegistrationRequest(userDetails);
                             _context.Add(request);
                             await _context.SaveChangesAsync();
                             return RedirectToPage("AwaitsApproval");
                        }
                        
                        var code =  await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Potvrdite Vašu e-mail adresu",
                            $"Poštovani,<br><br>molimo Vas da potvrdite Vašu registraciju na Hooking klikom na <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ovaj link</a>.");

                        var userCount = _userManager.Users.Count();
                        Console.WriteLine("Trenutni broj korisnika: " + userCount);
                        if (userCount == 1)
                        {
                            if (_roleManager.Roles.ToList().Count == 0)
                            {
                                IdentityRole role = new IdentityRole
                                {
                                    Name = "Admin"
                                };
                                await _roleManager.CreateAsync(role);
                                await _userManager.AddToRoleAsync(user, "Admin");
                                await _context.SaveChangesAsync();
                            } 
                        }

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
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
            if (Input.Type != RegistrationType.REGULAR)
            {
                return new IdentityUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true};
            }
            return new IdentityUser { UserName = Input.Email, Email = Input.Email};
        }
        private string GetInputType()
        {
            var roleName="";
            switch (Input.Type)
            {
                case RegistrationType.REGULAR:
                    roleName = "Korisnik";
                    break;
                case RegistrationType.BOAT_OWNER:
                    roleName = "Vlasnik broda";
                    break;
                case RegistrationType.COTTAGE_OWNER:
                    roleName = "Vlasnik vikendice";
                    break;
                case RegistrationType.INSTRUCTOR:
                    roleName = "Instruktor";
                    break;
            }

            return roleName;
        }
    }
}
