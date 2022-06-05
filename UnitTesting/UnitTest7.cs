using Hooking.Controllers;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    class UnitTest7
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        private  IEmailSender _emailSender;
        public CottageAppealsController cottageAppealsController;
        public Guid cottageId;
        public CottageAppeal cottageAppeal;
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Data Source=DESKTOP-CJ8VDR7;Initial Catalog=HookingDB;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;").Options;
            _context = new ApplicationDbContext(dbOption);
  
            cottageAppealsController = new CottageAppealsController(_context, _userManager, _emailSender);
            cottageId = Guid.Parse("cb93f47a-b7f3-4ca0-92a3-47594d811e5f");
            cottageAppeal = new CottageAppeal();
            cottageAppeal.AppealContent = "appeal content";
            cottageAppeal.UserEmail = "zenky1337@gmail.com";


        }
        #region Unit Test
        [Test]
        public async Task CreatingCottageAppeal()
        {
            await cottageAppealsController.Create(cottageId, cottageAppeal);
            var appeal = _context.CottageAppeal.Find(cottageAppeal.Id);
            Assert.IsInstanceOf<CottageAppeal>(appeal);

        }
        #endregion
    }
}
