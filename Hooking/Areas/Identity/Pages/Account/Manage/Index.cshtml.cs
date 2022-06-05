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

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Display(Name = "Korisničko ime")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Broj telefona")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Ime")]
            public string FirstName { get; set; }
            [Display(Name = "Prezime")]
            public string LastName { get; set; }
            [Display(Name = "Adresa")]
            public string Address { get; set; }
            [Display(Name = "Grad")]
            public string City { get; set; }
            [Display(Name = "Država")]
            public string Country { get; set; }

            [Display(Name = "Broj kaznenih poena")]
            public int PenaltyCount { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userDetails = _context.UserDetails.Where(x => x.IdentityUserId == user.Id).FirstOrDefault();

            Input = CreateInputFromUserDetails(userDetails);
            Input.PhoneNumber = phoneNumber;
            Username = userName;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nije moguće učitati korisnika čiji je ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nije moguće učitati korisnika čiji je ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Neočekivana greška prilikom promene broja telefona.";
                    return RedirectToPage();
                }
            }
            var userDetails = _context.UserDetails.FirstOrDefault(x => x.IdentityUserId == user.Id);
            bool UserDetailsChanged = CheckForChanges(userDetails);

            if (UserDetailsChanged)
            {
                _context.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Uspešno ažuriranje profila";
            return RedirectToPage();
        }


        public bool CheckForChanges(UserDetails userDetails)
        {
            bool UserDetailsChanged = false;
            if (Input.FirstName != userDetails.FirstName)
            {
                userDetails.FirstName = Input.FirstName;
                UserDetailsChanged = true;
            }

            if (Input.LastName!= userDetails.LastName)
            {
                userDetails.LastName= Input.LastName;
                UserDetailsChanged = true;
            }

            if (Input.Address != userDetails.Address)
            {
                userDetails.Address = Input.Address;
                UserDetailsChanged = true;
            }

            if (Input.City != userDetails.City)
            {
                userDetails.City = Input.City;
                UserDetailsChanged = true;
            }

            if (Input.Country != userDetails.Country)
            {
                userDetails.Country = Input.Country;
                UserDetailsChanged = true;
            }

            return UserDetailsChanged;
        }

        public InputModel CreateInputFromUserDetails(UserDetails userDetails)
        {
            return new InputModel
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                City = userDetails.City,
                Country = userDetails.Country
            };
        }
    }
}
