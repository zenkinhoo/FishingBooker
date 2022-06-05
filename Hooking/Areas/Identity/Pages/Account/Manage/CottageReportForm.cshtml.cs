using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hooking.Models;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public class CottageReportFormModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CottageReportFormModel(ApplicationDbContext context,
                                        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            var cottageOwner = _context.CottageOwner.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            var cottageOwnerId = cottageOwner.Id.ToString();
            List<Cottage> cottages = _context.Cottage.Where(m => m.CottageOwnerId == cottageOwnerId).ToList();
            List<string> cottageNames = new List<string>();
            List<double> cottageIncomes = new List<double>();
            List<int> cottageNumOfReservations = new List<int>();
            List<double> averageGrades = new List<double>();
            foreach(Cottage cottage in cottages)
            {
                string cottageId = cottage.Id.ToString();
                double income = 0;
                int reservations = 0;
                List<CottageReservation> cottageReservations = _context.CottageReservation.Where(m => m.CottageId == cottageId).ToList();
                foreach (CottageReservation cottageReservation in cottageReservations)
                {
                    if (cottageReservation.EndDate <= DateTime.Now)
                    {
                        income += cottageReservation.Price;
                        reservations++;
                    }
                }
                List<CottageSpecialOffer> cottageSpecialOffers = _context.CottageSpecialOffer.Where(m => m.CottageId == cottageId).ToList();
                foreach (CottageSpecialOffer cottageSpecialOffer in cottageSpecialOffers)
                {
                    if (cottageSpecialOffer.EndDate <= DateTime.Now && cottageSpecialOffer.IsReserved == true)
                    {
                        income += cottageSpecialOffer.Price;
                        reservations++;
                    }
                }
                cottageNames.Add(cottage.Name);
                cottageIncomes.Add(income);
                cottageNumOfReservations.Add(reservations);
                averageGrades.Add(cottage.AverageGrade);
            }
            ViewData["CottageNames"] = cottageNames;
            ViewData["CottageIncomes"] = cottageIncomes;
            ViewData["CottageNumOfReservations"] = cottageNumOfReservations;
            ViewData["TotalNumOfCottages"] = cottages.Count();
            ViewData["AverageGrades"] = averageGrades;
            return Page();
        }
    }
}
