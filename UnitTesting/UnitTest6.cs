using Hooking.Data;
using Hooking.Models;
using Hooking.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace UnitTesting
{
    class UnitTest6
    {
        private ApplicationDbContext _context;
        private FishingTechniquesController fishingTechniquesController;
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=tcp:hooking.database.windows.net,1433;Initial Catalog=HookingDB;Persist Security Info=False;User ID=pedja;Password=Omorika7212.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;").Options;
            _context = new ApplicationDbContext(dbOption);
            fishingTechniquesController = new FishingTechniquesController(_context);
        }
        [Test]
        public async Task CreateFishingTechniques()
        {
            FishingTechniques fishingTechniques = new FishingTechniques()
            {
                Id = Guid.NewGuid(),
                Inshore = true,
                InstructorHasBoat = true,
                Offshore = true
            };
            await fishingTechniquesController.Create(fishingTechniques);
            var temp = await _context.FishingTechniques.FindAsync(fishingTechniques.Id);
            Assert.IsInstanceOf<FishingTechniques>(temp);
        }
    }
}
