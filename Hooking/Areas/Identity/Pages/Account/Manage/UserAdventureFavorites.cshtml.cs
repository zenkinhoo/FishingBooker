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
    public class UserAdventureFavoritesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<AdventureFavorites> myAdventureFavorites { get; set; }
        public List<Adventure> myAdventures { get; set; }


        public UserAdventureFavoritesModel(UserManager<IdentityUser> userManager,
                             ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();

            myAdventureFavorites = await _context.AdventureFavorites.Where(m => m.UserDetailsId == userDetails.IdentityUserId.ToString()).ToListAsync();
            myAdventures = await _context.Adventure.ToListAsync();
            List<Adventure> adventureData = new List<Adventure>();
            List<Guid> adventureFavoritesId = new List<Guid>();

            foreach (AdventureFavorites adventureFavorite in myAdventureFavorites)
            {
                Guid guid = new Guid(adventureFavorite.AdventureId);
                Adventure ad = _context.Adventure.Where(m => m.Id == guid).FirstOrDefault<Adventure>();
                adventureData.Add(ad);
                adventureFavoritesId.Add(adventureFavorite.Id);

            }

            ViewData["AdventureData"] = adventureData;
            ViewData["AdventureFavoritesId"] = adventureFavoritesId;


            return Page();
        }
    }
}
