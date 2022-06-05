using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hooking.Data;
using Hooking.Models;
using Hooking.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx.Synchronous;

namespace Hooking.Services.Implementations
{
    public class AdventureService : IAdventureService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdventureService(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;

            using StreamReader reader = new StreamReader("./Data/emailCredentials.json");
            string json = reader.ReadToEnd();
            JsonConvert.DeserializeObject<EmailSender>(json);
            _userManager = userManager;
        }

        public IEnumerable<Adventure> GetAdventures()
        {
            return _context.Adventure.ToList();
        }

        public IEnumerable<AdventureReservationDTO> GetAdventureReservations(Guid instructorId)
        {

            var allAdventures = _context.Adventure.ToList();
            var allAdventureRealisations = _context.AdventureRealisation.ToList();
            var allAdventureReservations = _context.AdventureReservation.ToList();

            List<AdventureReservationDTO> retVal = new List<AdventureReservationDTO>();

            foreach (Adventure adventure in allAdventures)
            {
                foreach (AdventureRealisation realisation in allAdventureRealisations)
                {
                    foreach(AdventureReservation reservation in allAdventureReservations)
                    {
                        if (reservation.AdventureRealisationId == realisation.Id.ToString() &&
                            realisation.AdventureId == adventure.Id.ToString() && realisation.StartDate >= DateTime.Now)
                        {
                            AdventureReservationDTO dto = new AdventureReservationDTO(adventure, realisation, reservation);
                            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(dto.UserDetailsId));
                            if (userDetails == null)
                            {
                                continue;
                            }
                            dto.UserDetailsFirstName = userDetails.FirstName;
                            dto.UserDetailsLastName = userDetails.LastName;
                            retVal.Add(dto);
                        }
                    }
                }
            }


            return retVal;
        }
        public IEnumerable<AdventureReservationDTO> GetAdventureReservationsHistory(Guid instructorId)
        {

            var allAdventures = _context.Adventure.ToList();
            var allAdventureRealisations = _context.AdventureRealisation.ToList();
            var allAdventureReservations = _context.AdventureReservation.ToList();

            List<AdventureReservationDTO> retVal = new List<AdventureReservationDTO>();

            foreach (Adventure adventure in allAdventures)
            {
                foreach (AdventureRealisation realisation in allAdventureRealisations)
                {
                    foreach (AdventureReservation reservation in allAdventureReservations)
                    {
                        if (reservation.AdventureRealisationId == realisation.Id.ToString() &&
                            realisation.AdventureId == adventure.Id.ToString() && realisation.StartDate <= DateTime.Now)
                        {
                            AdventureReservationDTO dto = new AdventureReservationDTO(adventure, realisation, reservation);
                            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(dto.UserDetailsId));
                            if (userDetails == null)
                            {
                                continue;
                            }
                            dto.UserDetailsFirstName = userDetails.FirstName;
                            dto.UserDetailsLastName = userDetails.LastName;
                            retVal.Add(dto);
                        }
                    }
                }
            }

            return retVal;
        }

        public void AddAdventure(Adventure adventure)
        {
            adventure.Id = Guid.NewGuid();
            _context.Add(adventure);
            _context.SaveChanges();
        }

        public Adventure FindAdventureById(Guid id)
        {
            return _context.Adventure.Find(id);
        }

        public void UpdateAdventure(Adventure adventure)
        {
            Adventure updatedAdventure = FindAdventureById(adventure.Id);
            _context.Entry(updatedAdventure).CurrentValues.SetValues(adventure);
            _context.SaveChanges();
        }

        public void RemoveAdventure(Guid id)
        {
            var adventure = FindAdventureById(id);
            List<AdventureRealisation> adventureRealisations = new List<AdventureRealisation>();
            string adventureId = adventure.Id.ToString();
            adventureRealisations = GetAdventureRealiastions(id).ToList();
            if (adventureRealisations.Count == 0)
            {
                _context.Adventure.Remove(adventure);
                _context.SaveChanges();
            }
            List<AdventureSpecialOffer> adventureSpecialOffers = new List<AdventureSpecialOffer>();
            adventureSpecialOffers = _context.AdventureSpecialOffer.Where(m => m.AdventureId == adventureId).ToList();
            if (adventureSpecialOffers.Count == 0)
            {
                _context.Adventure.Remove(adventure);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Adventure> GetAdventuresForSpecialOffer(ClaimsPrincipal User)
        {
            var user = _userManager.GetUserAsync(User).WaitAndUnwrapException();

            Guid userId = Guid.Parse(user.Id);
            System.Diagnostics.Debug.WriteLine(userId);
            UserDetails userDetails = _context.UserDetails.Where(m => m.IdentityUserId == user.Id).FirstOrDefault();
            var userDetailsId = userDetails.Id.ToString();
            Instructor instructor = _context.Instructor.Where(m => m.UserDetailsId == userDetailsId).FirstOrDefault();
            string instructorId = instructor.Id.ToString();
            List<Adventure> adventures = _context.Adventure.Where(m => m.InstructorId == instructorId).ToList();
            return adventures;
        }

        public IEnumerable<AdventureImage> GetAdventureImages(Guid adventureId)
        {
            return _context.AdventureImage.Where(m => m.AdventureId == adventureId.ToString()).ToList();
        }

        public void AddAdventureImage(Guid adventureId, CloudBlockBlob blockBlob)
        {
            AdventureImage adventureImage = new AdventureImage();
            adventureImage.Id = Guid.NewGuid();
            adventureImage.AdventureId = adventureId.ToString();
            adventureImage.ImageUrl = blockBlob.Uri.ToString();
            _context.AdventureImage.Add(adventureImage);
            _context.SaveChanges();
        }

        public IEnumerable<AdventureDTO> GetInstructorAdventures(string userId)
        {
            Guid userDetailsId = GetUserDetailsIdFromUserId(userId);

            Guid instructorId = GetInstructorIdFromUserDetailsId(userDetailsId);

            IEnumerable<Adventure> adventures = _context.Adventure.Where(a => a.InstructorId == instructorId.ToString()).ToList();
            List<AdventureDTO> dtos = new List<AdventureDTO>();

            foreach (Adventure adventure in adventures)
            {
                AdventureDTO tempDto = new AdventureDTO(adventure);
                
                CancelationPolicy policy = _context.CancelationPolicy.Find(Guid.Parse(adventure.CancellationPolicyId));
                tempDto.PopulateFieldsFromCancellationPolicy(policy);

                tempDto.InstructorName = GetInstructorNameFromId(instructorId);
                tempDto.InstructorBiography = _context.Instructor.Find(instructorId).Biography;
                dtos.Add(tempDto);
            }
            
            return dtos;
        }

        public bool AdventureEditable(Guid adventureId)
        {
            List<AdventureRealisation> realisations = _context.AdventureRealisation.Where(a => a.AdventureId == adventureId.ToString() && DateTime.Now >= a.StartDate).ToList();
            if (realisations.Count == 0)
            {
                return true;
            }

            foreach (AdventureRealisation realisation in realisations)
            {
                AdventureReservation reservation = _context.AdventureReservation.FirstOrDefault(r => r.AdventureRealisationId == realisation.Id.ToString());

                if (reservation == null)
                {
                    return true;
                }
            }
            
            return false;
        }

        public AdventureDTO GetAdventureDto(Guid adventureId)
        {
            Adventure adventure = _context.Adventure.Find(adventureId);
            AdventureDTO dto = new AdventureDTO(adventure);
            CancelationPolicy policy = _context.CancelationPolicy.Find(Guid.Parse(adventure.CancellationPolicyId));
            dto.PopulateFieldsFromCancellationPolicy(policy);

            dto.InstructorName = GetInstructorNameFromId(Guid.Parse(adventure.InstructorId));
            dto.InstructorBiography = _context.Instructor.Find(Guid.Parse(adventure.InstructorId)).Biography;

            var adventureEquipment =
                _context.AdventureFishingEquipment.FirstOrDefault(e => e.AdventureId == adventureId.ToString());


            System.Diagnostics.Debug.WriteLine("prosledjena avantura id " + adventureId.ToString());


            string rulesId = _context.AdventuresAdventureRules
                .FirstOrDefault(r => r.AdventureId == adventureId.ToString()).AdventureRulesId;

            AdventureRules rules = _context.AdventureRules.Find(Guid.Parse(rulesId));

            if (adventureEquipment == null)
            {
                dto.PopulateFieldsFromRulesWithoutFishing(rules);
                return dto;
            }
            FishingEquipment equipment = _context.FishingEquipment.FirstOrDefault(e => e.Id == Guid.Parse(adventureEquipment.FishingEquipmentId));
            dto.PopulateFieldsFromFishingEquipment(equipment);
            dto.PopulateFieldsFromRulesWithFishing(rules);
            return dto;
        }

        private AdventureRules FindExistingRules(AdventureDTO dto)
        {
            return _context.AdventureRules.FirstOrDefault(r =>
                r.CabinSmoking == dto.CabinSmoking && r.CatchAndReleaseAllowed == dto.CatchAndReleaseAllowed &&
                r.ChildFriendly == dto.ChildFriendly && r.YouKeepCatch == dto.YouKeepCatch);
        }

        public void Create(AdventureDTO dto)
        {
            Adventure adventure = new Adventure(dto);
            adventure.Id = Guid.NewGuid();
            _context.Add(adventure);

            AdventureRules rules = FindExistingRules(dto);

            if (rules == null)
            {
                rules = new AdventureRules
                {
                    CabinSmoking = dto.CabinSmoking,
                    CatchAndReleaseAllowed = dto.CatchAndReleaseAllowed,
                    ChildFriendly = dto.ChildFriendly,
                    YouKeepCatch = dto.YouKeepCatch
                };
                _context.Add(rules);
            }

            _context.Add(new AdventuresAdventureRules
            {
                AdventureId = adventure.Id.ToString(),
                AdventureRulesId = rules.Id.ToString()
            });

            _context.SaveChanges();
        }

        public bool Subscribe(Guid adventureId, AdventureFavorites favorite, IdentityUser identityUser)
        {
            favorite.Id = Guid.NewGuid();
            favorite.AdventureId = adventureId.ToString();

            UserDetails userDetails = _context.UserDetails.FirstOrDefault(u => u.IdentityUserId == identityUser.Id);

            favorite.UserDetailsId = userDetails.IdentityUserId.ToString();

            Adventure adventure = _context.Adventure.Where(m => m.Id == adventureId).FirstOrDefault();
            adventure.hasSubscribers = true;
            try
            {
                _context.Update(adventure);
                _context.SaveChanges();

                _context.Add(favorite);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                System.Diagnostics.Debug.WriteLine("bacam exception");
                return false;
            }

        }

        public IEnumerable<UserDetails> GetAllUserDetails()
        {
            return _context.UserDetails.ToList();
        }

        public IEnumerable<AdventureRealisation> GetAdventureRealiastions(Guid id)
        {
            return _context.AdventureRealisation.Where(m => m.AdventureId == id.ToString()).ToList();
        }

        public bool AdventureExists(Guid id)
        {
            return _context.Adventure.Any(e => e.Id == id);
        }

        private string GetInstructorNameFromId(Guid instructorId)
        {
            Instructor instructor = _context.Instructor.Find(instructorId);
            UserDetails userDetails = _context.UserDetails.Find(Guid.Parse(instructor.UserDetailsId));

            return $"{userDetails.FirstName} {userDetails.LastName}";
        }

        private Guid GetUserDetailsIdFromUserId(string userId)
        {
            return _context.UserDetails.Where(user => user.IdentityUserId == userId)
                .Select(user => user.Id)
                .FirstOrDefault();
        }

        private Guid GetInstructorIdFromUserDetailsId(Guid userDetailsId)
        {
            return _context.Instructor.Where(inst => inst.UserDetailsId == userDetailsId.ToString())
                .Select(inst => inst.Id)
                .FirstOrDefault();
        }
    }
}
