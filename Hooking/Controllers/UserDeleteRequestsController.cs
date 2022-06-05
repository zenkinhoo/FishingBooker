using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hooking.Data;
using Hooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Hooking.Models.DTO;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using OpenQA.Selenium.DevTools.V85.Debugger;
using OpenQA.Selenium.Interactions;

namespace Hooking.Controllers
{
    public class UserDeleteRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        [TempData]
        public string StatusMessage { get; set; }
        public UserDeleteRequestsController(ApplicationDbContext context,
                                            UserManager<IdentityUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            SignInManager<IdentityUser> signInManager,
                                            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            using (StreamReader reader = new StreamReader("./Data/emailCredentials.json"))
            {
                string json = reader.ReadToEnd();
                _emailSender = JsonConvert.DeserializeObject<EmailSender>(json);
            }
        }

        // GET: UserDeleteRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.UserDeleteRequest.ToListAsync();
            return View(requests.OrderBy(r => r.isReviewed));
        }

        public async Task<IActionResult> AnswerRequest(DeleteRequestDTO dto)
        {
            return View(dto);
        }

        public async Task<IActionResult> GrantRequest(Guid id)
        {
            UserDeleteRequest request = _context.UserDeleteRequest.Find(id);
            return RedirectToAction(nameof(AnswerRequest),new DeleteRequestDTO
            {
                UserDetailsId = request.UserDetailsId,
                Type = request.Type,
                IsApproved = true
            });
        }

        public async Task<IActionResult> DenyRequest(Guid id)
        {
            UserDeleteRequest request = _context.UserDeleteRequest.Find(id);
            return RedirectToAction(nameof(AnswerRequest), new DeleteRequestDTO
            {
                UserDetailsId = request.UserDetailsId,
                Type = request.Type,
                
                IsApproved = false
            });
        }

        private string GetEmailFromUserDetailsId(string userDetailsId)
        {
            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(userDetailsId));
            IdentityUser iUser = _userManager.FindByIdAsync(userDetails.IdentityUserId).WaitAndUnwrapException();
            if (iUser != null) return iUser.Email;
            return "";
        }
        private void DeleteBoatOwner(UserDeleteRequest request)
        {
            BoatOwner boatOwner = _context.BoatOwner.FirstOrDefault(i => i.UserDetailsId == request.UserDetailsId);
            _context.Remove(boatOwner);
            foreach (Boat boat in _context.Boat.Where(b => b.BoatOwnerId == boatOwner.Id.ToString()))
            {
                foreach(BoatReservation reservation in _context.BoatReservation.Where(r => r.StartDate < DateTime.Now).ToList())
                {
                    _context.Remove(reservation);
                    _emailSender.SendEmailAsync(GetEmailFromUserDetailsId(reservation.UserDetailsId), "Otkazivanje rezervacija", $"Nažalost, sve buduće rezervacije u vezi sa brodom {boat.Name} su otkazane zbog brisanja profila vlasnika.")
                        .WaitAndUnwrapException();
                }
            }
            
            _context.SaveChanges();
        }
        private void DeleteCottageOwner(UserDeleteRequest request)
        {
            CottageOwner cottageOwner = _context.CottageOwner.FirstOrDefault(i => i.UserDetailsId == request.UserDetailsId);
            _context.Remove(cottageOwner);
            foreach (Cottage cottage in _context.Cottage.Where(b => b.CottageOwnerId == cottageOwner.Id.ToString()))
            {
                foreach (CottageReservation reservation in _context.CottageReservation.Where(r => r.StartDate < DateTime.Now).ToList())
                {
                    _context.Remove(reservation);
                    _emailSender.SendEmailAsync(GetEmailFromUserDetailsId(reservation.UserDetailsId), "Otkazivanje rezervacija", $"Nažalost, sve buduće rezervacije u vezi sa brodom {cottage.Name} su otkazane zbog brisanja profila vlasnika.")
                        .WaitAndUnwrapException();
                }
            }

            _context.SaveChanges();
        }
        private void DeleteInstructorOwner(UserDeleteRequest request)
        {
            Instructor instructor = _context.Instructor.FirstOrDefault(i => i.UserDetailsId == request.UserDetailsId);
            _context.Remove(instructor);
            foreach (Adventure adventure in _context.Adventure.Where(b => b.InstructorId == instructor.Id.ToString()))
            {
                foreach (AdventureRealisation realization in _context.AdventureRealisation.Where(r => r.StartDate < DateTime.Now).ToList())
                {
                    _context.Remove(realization);
                    foreach (AdventureReservation reservation in _context.AdventureReservation.Where(r =>
                        r.AdventureRealisationId == realization.Id.ToString()))
                    {
                        _emailSender.SendEmailAsync(GetEmailFromUserDetailsId(reservation.UserDetailsId), "Otkazivanje rezervacija", $"Nažalost, sve buduće rezervacije u vezi sa brodom {adventure.Name} su otkazane zbog brisanja profila vlasnika.").WaitAndUnwrapException();
                        _context.Remove(reservation);
                    }
                }
            }

            _context.SaveChanges();
        }

        public async Task<IActionResult> RegisterAnswer([Bind("Description,IsApproved,Type,UserDetailsId")]
            DeleteRequestDTO dto)
        {
            UserDeleteRequest request = _context.UserDeleteRequest.FirstOrDefault(r => r.UserDetailsId == dto.UserDetailsId && r.isReviewed == false);
            
            if (request != null) request.IsApproved = dto.IsApproved;
            else return NotFound();
            
            UserDetails userDetails = await _context.UserDetails.FindAsync(Guid.Parse(request.UserDetailsId));
            string message = "";
            if (request.IsApproved)
            {
                switch (request.Type)
                {
                    case DeletionType.BOATOWNER:
                        DeleteBoatOwner(request);
                        break;
                    case DeletionType.COTTAGEOWNER:
                        DeleteCottageOwner(request);
                        break;
                    case DeletionType.INSTRUCTOR:
                        DeleteInstructorOwner(request);
                        break;
                }
                _context.Remove(userDetails);
                message = "Vaš zahtev za brisanje je odobren.";
            }
            else
            {
                message = "Vaš zahtev za brisanje nije odobren.";
            }

            request.isReviewed = true;

            await _emailSender.SendEmailAsync(GetEmailFromUserDetailsId(userDetails.Id.ToString()), "Zahtev za brisanje", message);
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

        // GET: UserDeleteRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDeleteRequest = await _context.UserDeleteRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDeleteRequest == null)
            {
                return NotFound();
            }

            return View(userDeleteRequest);
        }

        // GET: UserDeleteRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserDeleteRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Id,RowVersion")] UserDeleteRequest userDeleteRequest)
        {
            Console.WriteLine("usao u create");
            if (ModelState.IsValid)
            {
                Console.WriteLine("model je validan");

                var user = await _userManager.GetUserAsync(User);
                Console.WriteLine("id usera"+user.Id.ToString());

                List<UserDeleteRequest> userDeleteRequests = _context.UserDeleteRequest.Where(m => m.UserDetailsId == user.Id).ToList();
                foreach(var userDeleteRequestTemp in userDeleteRequests)
                {
                    if(!userDeleteRequestTemp.isReviewed)
                    {
                        StatusMessage = "Error: Ne možete poslati više od jednog zahteva za brisanje profila.";
                        return RedirectToPage("/Account/Manage/UserDeleteRequest", new { area = "Identity" });
                    }
                }

                UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == user.Id);
                Console.WriteLine("id userdetailsa" + userDetails.Id.ToString());

                userDeleteRequest.Id = Guid.NewGuid();
                userDeleteRequest.IsApproved = false;
                userDeleteRequest.UserDetailsId = userDetails.Id.ToString();
                userDeleteRequest.isReviewed = false;
                IList<string> rolenames = await _signInManager.UserManager.GetRolesAsync(user);
                switch (rolenames[0])
                {
                    case "Vlasnik vikendice":
                        userDeleteRequest.Type = DeletionType.COTTAGEOWNER;
                        break;
                    case "Korisnik":
                        Console.WriteLine("usao u case korisnik");

                        userDeleteRequest.Type = DeletionType.USER;
                        break;
                    case "Instruktor":
                        userDeleteRequest.Type = DeletionType.INSTRUCTOR;
                        break;
                    case "Vlasnik broda":
                        userDeleteRequest.Type = DeletionType.BOATOWNER;
                        break;
                    case "Admin":
                        userDeleteRequest.Type = DeletionType.ADMIN;
                        break;
                }
                Console.WriteLine("tik sam pred dodavanje");

                _context.Add(userDeleteRequest);
                await _context.SaveChangesAsync();
                
                await _emailSender.SendEmailAsync(user.Email, "Potvrda poslatog zahteva za brisanje profila",
                            $"Poštovani, Obaveštavamo Vas da smo primili Vaš zahtev za brisanje naloga. U narednom periodu ćete biti obavešteni o odluci admin tima. Hvala na strpljenju!");

                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }
            return View(userDeleteRequest);
        }

        // GET: UserDeleteRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDeleteRequest = await _context.UserDeleteRequest.FindAsync(id);
            if (userDeleteRequest == null)
            {
                return NotFound();
            }
            return View(userDeleteRequest);
        }

        // POST: UserDeleteRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserDetailsId,Description,IsApproved,Type,Id,RowVersion")] UserDeleteRequest userDeleteRequest)
        {
            if (id != userDeleteRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userDeleteRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDeleteRequestExists(userDeleteRequest.Id))
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
            return View(userDeleteRequest);
        }

        // GET: UserDeleteRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDeleteRequest = await _context.UserDeleteRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDeleteRequest == null)
            {
                return NotFound();
            }

            return View(userDeleteRequest);
        }

        // POST: UserDeleteRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userDeleteRequest = await _context.UserDeleteRequest.FindAsync(id);
            _context.UserDeleteRequest.Remove(userDeleteRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserDeleteRequestExists(Guid id)
        {
            return _context.UserDeleteRequest.Any(e => e.Id == id);
        }
    }
}
