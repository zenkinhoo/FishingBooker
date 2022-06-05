using Hooking.Data;
using Hooking.Models;
using Hooking.Controllers;
using Hooking.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace UnitTesting
{
    class UnitTest5
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public AdventureService adventureService;
        public AdventuresController adventuresController;
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseSqlServer("Server=tcp:hooking.database.windows.net,1433;Initial Catalog=HookingDB;Persist Security Info=False;User ID=pedja;Password=Omorika7212.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;").Options;
            _context = new ApplicationDbContext(dbOption);
            adventureService = new AdventureService(_context, _userManager);
            adventuresController = new AdventuresController(adventureService, _context);
        }
        [Test]
        public async Task Test()
        {
            Adventure adventure = await _context.Adventure.FirstOrDefaultAsync();
            int maxPerson = adventure.MaxPersonCount;
            adventure.MaxPersonCount = maxPerson + 2;
            var actionResult = adventuresController.Edit(adventure.Id, adventure);
            Assert.AreNotEqual(maxPerson, adventure.MaxPersonCount);
        }
    }
}
