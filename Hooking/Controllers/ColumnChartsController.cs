using Hooking.Data;
using Hooking.Models;
using Hooking.Models.DTO;
using Hooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Controllers
{
    public class ColumnChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAdventureService _adventureService;
        public ColumnChartsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IAdventureService adventureService)
        {
            _context = context;
            _userManager = userManager;
            _adventureService = adventureService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<List<ColumnChart>> GetListYearAsync()
        {
            var list = new List<ColumnChart>();
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var instructor = _context.Instructor.Where(m => m.UserDetailsId == userDetails.Id.ToString()).FirstOrDefault();
            var instructorId = instructor.Id.ToString();
            List<AdventureReservationDTO> adventureReservations = _adventureService.GetAdventureReservations(instructor.Id).ToList();
            var history = _adventureService.GetAdventureReservationsHistory(instructor.Id);
            adventureReservations.AddRange(history);
            double income2022 = 0;
            double income2021 = 0;
            foreach(AdventureReservationDTO adventureReservationDTO in adventureReservations)
            {
                var rId = Guid.Parse(adventureReservationDTO.AdventureRealisationId);
                var realisation = _context.AdventureRealisation.Where(m => m.Id == rId).FirstOrDefault();
                if(realisation.StartDate.Year == 2021)
                {
                    income2021 += realisation.Price;
                } else
                {
                    income2022 += realisation.Price;
                }

            }
            list.Add(new ColumnChart { AdventureName = "2022", Income =  income2022}); 
            list.Add(new ColumnChart { AdventureName = "2021", Income =  income2021});
            

            return list;
        }

        public async Task<List<ColumnChart>> GetListQuarterAsync()
        {
            var list = new List<ColumnChart>();
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var instructor = _context.Instructor.Where(m => m.UserDetailsId == userDetails.Id.ToString()).FirstOrDefault();
            var instructorId = instructor.Id.ToString();
            List<AdventureReservationDTO> adventureReservations = _adventureService.GetAdventureReservations(instructor.Id).ToList();
            var history = _adventureService.GetAdventureReservationsHistory(instructor.Id);
            adventureReservations.AddRange(history);
            double income1 = 0;
            double income2 = 0;
            double income3 = 0;
            double income4 = 0;
            foreach (AdventureReservationDTO adventureReservationDTO in adventureReservations)
            {
                var rId = Guid.Parse(adventureReservationDTO.AdventureRealisationId);
                var realisation = _context.AdventureRealisation.Where(m => m.Id == rId).FirstOrDefault();
                if(realisation.StartDate.Year == 2021)
                {
                    if(realisation.StartDate.Month >= 1 && realisation.StartDate.Month <=3)
                    {
                        income1 += realisation.Price;
                    } else if(realisation.StartDate.Month >=4 && realisation.StartDate.Month <=6)
                    {
                        income2 += realisation.Price;
                    } else if(realisation.StartDate.Month >=7 && realisation.StartDate.Month <=9)
                    {
                        income3 += realisation.Price;
                    } else
                    {
                        income4 += realisation.Price;
                    }

                }
            }
            list.Add(new ColumnChart { AdventureName = "I Kvartal", Income = income1 });
            list.Add(new ColumnChart { AdventureName = "II Kvartal", Income = income2 });
            list.Add(new ColumnChart { AdventureName = "III Kvartal", Income = income3 });
            list.Add(new ColumnChart { AdventureName = "IV Kvartal", Income = income4 });

            return list;
        }

        public async Task<List<ColumnChart>> GetListMonthAsync()
        {
            var list = new List<ColumnChart>();
            
            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);
            var userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var instructor = _context.Instructor.Where(m => m.UserDetailsId == userDetails.Id.ToString()).FirstOrDefault();
            var instructorId = instructor.Id.ToString();
            List<AdventureReservationDTO> adventureReservations = _adventureService.GetAdventureReservations(instructor.Id).ToList();
            var history = _adventureService.GetAdventureReservationsHistory(instructor.Id);
            adventureReservations.AddRange(history);
            double income1 = 0;
            double income2 = 0;
            double income3 = 0;
            double income4 = 0;
            double income5 = 0;
            double income6 = 0;
            double income7 = 0;
            double income8 = 0;
            double income9 = 0;
            double income10 = 0;
            double income11 = 0;
            double income12 = 0;
            foreach (AdventureReservationDTO adventureReservationDTO in adventureReservations)
            {
                var rId = Guid.Parse(adventureReservationDTO.AdventureRealisationId);
                var realisation = _context.AdventureRealisation.Where(m => m.Id == rId).FirstOrDefault();
                if (realisation.StartDate.Year == 2021)
                {
                    if(realisation.StartDate.Month == 1)
                    {
                        income1 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 2)
                    {
                        income2 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 3)
                    {
                        income3 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 4)
                    {
                        income4 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 5)
                    {
                        income5 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 6)
                    {
                        income6 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 7)
                    {
                        income7 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 8)
                    {
                        income8 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 9)
                    {
                        income9 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 10)
                    {
                        income10 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 11)
                    {
                        income11 += realisation.Price;
                    }
                    if (realisation.StartDate.Month == 12)
                    {
                        income12 += realisation.Price;
                    }
                }
            }

            list.Add(new ColumnChart { AdventureName = "Januar", Income = income1 });
            list.Add(new ColumnChart { AdventureName = "Februar", Income = income2 });
            list.Add(new ColumnChart { AdventureName = "Mart", Income = income3 });
            list.Add(new ColumnChart { AdventureName = "April", Income = income4 });
            list.Add(new ColumnChart { AdventureName = "Maj", Income = income5 });
            list.Add(new ColumnChart { AdventureName = "Jun", Income = income6 });
            list.Add(new ColumnChart { AdventureName = "Jul", Income = income7 });
            list.Add(new ColumnChart { AdventureName = "Avgust", Income = income8 });
            list.Add(new ColumnChart { AdventureName = "Septembar", Income = income9 });
            list.Add(new ColumnChart { AdventureName = "Oktobar", Income = income10 });
            list.Add(new ColumnChart { AdventureName = "Novembar", Income = income11 });
            list.Add(new ColumnChart { AdventureName = "Decembar", Income = income12 });
            return list;
        }

        [HttpGet]
        public JsonResult ChartYear()
        {
            var populationList = GetListYearAsync().Result;
            return Json(populationList);
        }

        [HttpGet]
        public JsonResult ChartQuarter()
        {
            var populationList = GetListQuarterAsync().Result;
            return Json(populationList);
        }

        [HttpGet]
        public JsonResult ChartMonth()
        {
            var populationList = GetListMonthAsync().Result;
            return Json(populationList);
        }

    }
}
