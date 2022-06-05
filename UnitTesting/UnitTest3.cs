using NUnit.Framework;
using Hooking.Controllers;
using Microsoft.AspNetCore.Identity;
using Hooking.Data;
using Hooking.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTesting
{
    class UnitTest3
    {
        private ApplicationDbContext _context;
        public HouseRulesController houseRulesController; 
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=tcp:hooking.database.windows.net,1433;Initial Catalog=HookingDB;Persist Security Info=False;User ID=pedja;Password=Omorika7212.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;").Options;
            _context = new ApplicationDbContext(dbOption);
            houseRulesController = new HouseRulesController(_context);
        }
        #region Unit Test
        [Test]
        public async Task EditHouseRules()
        {
            HouseRules houseRules = await _context.HouseRules.FirstOrDefaultAsync();
            int ageRestriction = houseRules.AgeRestriction;
            houseRules.AgeRestriction = houseRules.AgeRestriction + 10;
            var actionResult = houseRulesController.Edit(houseRules.Id, houseRules);
            Assert.AreNotEqual(ageRestriction, houseRules.AgeRestriction);
        }
        #endregion
    }
}
