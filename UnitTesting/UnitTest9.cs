using Hooking.Controllers;
using Hooking.Data;
using Hooking.Data.Migrations;
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
    class UnitTest9
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public BoatFavoritesController boatFavoritesController;
        public BoatFavorites boatFavorites;
        public string boatId;
        [SetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Data Source=DESKTOP-CJ8VDR7;Initial Catalog=HookingDB;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;").Options;
            _context = new ApplicationDbContext(dbOption);

            boatFavoritesController = new BoatFavoritesController(_context, _userManager);
            boatId = "76327af1-f2c8-40bb-b202-96fb51079d79";



        }
        #region Unit Test
        [Test]
        public void boatExists()
        {
            bool exists = boatFavoritesController.BoatExists(boatId);
            Assert.IsTrue(exists);
        }
        #endregion
    }
}
