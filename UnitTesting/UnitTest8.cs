using Hooking.Controllers;
using Hooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    class UnitTest8
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private  IDistributedCache _cache;

        public CottagesController cottagesController;
        DateTime startDate1;
        DateTime endDate1;

        DateTime startDate2;
        DateTime endDate2;
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Data Source=DESKTOP-CJ8VDR7;Initial Catalog=HookingDB;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;").Options;
            _context = new ApplicationDbContext(dbOption);

            cottagesController = new CottagesController(_context, _userManager, _roleManager, _cache);

            startDate1 = new DateTime(2022, 8, 18);
            endDate1 = new DateTime(2022, 8, 23);

            startDate2 = new DateTime(2022, 8, 19);
            endDate2 = new DateTime(2022, 8, 25);


        }
        #region Unit Test
        [Test]
        public void areDatesOverlapping()
        {
            bool isAvaliable = cottagesController.isCottageAvailable(startDate1, endDate1, startDate2, endDate2);
            Assert.IsFalse(isAvaliable);
;

        }
        #endregion
    }
}
