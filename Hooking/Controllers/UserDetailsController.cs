using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;

namespace Hooking.Controllers
{
    public class UserDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       
        public UserDetailsController(ApplicationDbContext context,
                                     UserManager<IdentityUser> userManager,
                                     RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: UserDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserDetails.ToListAsync());
        }

        // GET: UserDetails/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetails = await _context.UserDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDetails == null)
            {
                return NotFound();
            }

            return View(userDetails);
        }

        // GET: UserDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdentityUserId,FirstName,LastName,Address,City,Country,PenaltyCount,Id,RowVersion")] UserDetails userDetails)
        {
            if (ModelState.IsValid)
            {
                userDetails.Id = Guid.NewGuid();
                _context.Add(userDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userDetails);
        }

        // GET: UserDetails/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetails = await _context.UserDetails.FindAsync(id);
            if (userDetails == null)
            {
                return NotFound();
            }
            return View(userDetails);
        }
        [HttpGet("/Users/ShowAllUsers/{id}")]
        public async Task<IActionResult> ShowAllUsers(Guid id)
        {
            
            string cId = id.ToString();
            List<UserDetails> userDetails = new List<UserDetails>();
            List<CottageReservation> cottageReservations = _context.CottageReservation.Where(m => m.CottageId == cId).ToList();
            foreach (CottageReservation cottageReservation in cottageReservations)
            {
                if (cottageReservation.StartDate <= DateTime.Now && cottageReservation.EndDate >= DateTime.Now)
                {
                    UserDetails user = _context.UserDetails.Where(m => m.IdentityUserId == cottageReservation.UserDetailsId).FirstOrDefault<UserDetails>();
                    userDetails.Add(user);
                }
            }
            ViewData["CottageId"] = id;
            return View(userDetails);
        }
        [HttpGet("/Users/ShowUsers/{id}")]
        public async Task<IActionResult> ShowUsers(Guid id)
        {
            string bId = id.ToString();
            List<UserDetails> userDetails = new List<UserDetails>();
            List<BoatReservation> boatReservations = _context.BoatReservation.Where(m => m.BoatId == bId).ToList();
            foreach(BoatReservation boatReservation in boatReservations)
            {
                if(boatReservation.StartDate <= DateTime.Now && boatReservation.EndDate >= DateTime.Now)
                {
                    UserDetails user = _context.UserDetails.Where(m => m.IdentityUserId == boatReservation.UserDetailsId).FirstOrDefault();
                    userDetails.Add(user);
                }
            }
            ViewData["BoatId"] = id;
            return View(userDetails);
        }
        // POST: UserDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IdentityUserId,FirstName,LastName,Address,City,Country,PenaltyCount,Id,RowVersion")] UserDetails userDetails)
        {
            if (id != userDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDetailsExists(userDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userDetails);
        }

        // GET: UserDetails/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetails = await _context.UserDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDetails == null)
            {
                return NotFound();
            }

            return View(userDetails);
        }

        // POST: UserDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userDetails = await _context.UserDetails.FindAsync(id);
            _context.UserDetails.Remove(userDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserDetailsExists(Guid id)
        {
            return _context.UserDetails.Any(e => e.Id == id);
        }
    }
}
