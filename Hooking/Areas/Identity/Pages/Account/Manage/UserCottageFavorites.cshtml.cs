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
    public class UserCottageFavoritesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<CottageFavorites> myCottageFavorites { get; set; }
        public List<Cottage> myCottages { get; set; }


        public UserCottageFavoritesModel(UserManager<IdentityUser> userManager,
                             ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            
            myCottageFavorites = await _context.CottageFavorites.Where(m => m.UserDetailsId == userDetails.IdentityUserId.ToString()).ToListAsync();
            myCottages = await _context.Cottage.ToListAsync();
            List<Cottage> cottageData = new List<Cottage>();
            List<Guid> cottageFavoritesId = new List<Guid>();
            foreach(CottageFavorites cottageFavorite in myCottageFavorites)
            {
                Guid guid = new Guid(cottageFavorite.CottageId);
                Cottage ctg = _context.Cottage.Where(m =>  m.Id == guid).FirstOrDefault<Cottage>();
                cottageData.Add(ctg);
                cottageFavoritesId.Add(cottageFavorite.Id);
            }

            ViewData["CottageData"] = cottageData;
            ViewData["CottageFavoritesId"] = cottageFavoritesId;

            return Page();
        }
    }
}
