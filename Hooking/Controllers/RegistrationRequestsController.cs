using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;

namespace Hooking.Controllers
{
    public class RegistrationRequestsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RegistrationRequestsController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: RegistrationRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.RegistrationRequest.ToListAsync());
        }
        
        public async Task<IActionResult> Approve(Guid id)
        {
            var request = await _context.RegistrationRequest.FindAsync(id);
            if (request == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.RegistrationRequest.Remove(request);

            UserDetails userDetails = await _context.UserDetails.FindAsync(Guid.Parse(request.UserDetailsId));
            userDetails.Approved = true;

            switch (request.Type)
            {
                case RegistrationType.COTTAGE_OWNER:
                {
                    CottageOwner cottageOwner = CreateCottageOwner(userDetails);
                    _context.Add(cottageOwner);
                    break;
                }
                case RegistrationType.BOAT_OWNER:
                {
                    BoatOwner boatOwner = createBoatOwner(userDetails);
                    _context.Add(boatOwner);
                    break;
                }
                case RegistrationType.INSTRUCTOR:
                {
                    Instructor instructor = CreateInstructor(userDetails);
                    _context.Add(instructor);
                    break;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(Guid id)
        {
            var request = await _context.RegistrationRequest.FindAsync(id);
            if (request == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.RegistrationRequest.Remove(request);
            UserDetails userDetails = await _context.UserDetails.FindAsync(Guid.Parse(request.UserDetailsId));
            if (userDetails == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            _context.UserDetails.Remove(userDetails);

            IdentityUser user = await _userManager.FindByIdAsync(userDetails.IdentityUserId);
            if (user == null)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }
            await _userManager.DeleteAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                Debug.WriteLine("Concurrency error!");
                return RedirectToAction("ConcurrencyError", "Home");
            }

            return RedirectToAction(nameof(Index));
        }

        private CottageOwner CreateCottageOwner(UserDetails userDetails)
        {
            CottageOwner cottageOwner = new CottageOwner
            {
                Id = Guid.NewGuid(),
                UserDetailsId = userDetails.Id.ToString(),
                AverageGrade = 0,
                GradeCount = 0
            };
            return cottageOwner;
        }

        private BoatOwner createBoatOwner(UserDetails userDetails)
        {
            BoatOwner boatOwner = new BoatOwner
            {
                Id = Guid.NewGuid(),
                UserDetailsId = userDetails.Id.ToString(),
                AverageGrade = 0,
                GradeCount = 0,
                IsCaptain = false,
                IsFirstOfficer = false
            };
            return boatOwner;
        }

        private Instructor CreateInstructor(UserDetails userDetails)
        {
            Instructor instructor = new Instructor
            {
                Id = Guid.NewGuid(),
                UserDetailsId = userDetails.Id.ToString(),
                AverageGrade = 0,
                Biography = "",
                GradeCount = 0
            };
            return instructor;
        }
    }
}
