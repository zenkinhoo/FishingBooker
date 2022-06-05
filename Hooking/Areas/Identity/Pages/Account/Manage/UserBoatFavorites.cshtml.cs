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
    public class UserBoatFavoritesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<BoatFavorites> myBoatFavorites { get; set; }

        public UserBoatFavoritesModel(UserManager<IdentityUser> userManager,
                            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();

            myBoatFavorites = await _context.BoatFavorites.Where(m => m.UserDetailsId == userDetails.IdentityUserId.ToString()).ToListAsync();
        //    myCottages = await _context.Cottage.ToListAsync();
            List<Boat> boatData = new List<Boat>();
            List<Guid> boatFavoritesId = new List<Guid>();

            foreach (BoatFavorites boatFavorite in myBoatFavorites)
            {
                Guid guid = new Guid(boatFavorite.BoatId);
                Boat bt = _context.Boat.Where(m => m.Id == guid).FirstOrDefault<Boat>();
                boatData.Add(bt);
                boatFavoritesId.Add(boatFavorite.Id);

            }

            ViewData["BoatData"] = boatData;
            ViewData["BoatFavoritesId"] = boatFavoritesId;


            return Page();
        }
    }
}
