using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hooking.Models;
using Hooking.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Hooking.Services
{
    public interface IAdventureService
    {
        IEnumerable<Adventure> GetAdventures();
        IEnumerable<AdventureReservationDTO> GetAdventureReservations(Guid instructorId);
        IEnumerable<AdventureReservationDTO> GetAdventureReservationsHistory(Guid instructorId);
        Adventure FindAdventureById(Guid id);
        void UpdateAdventure(Adventure adventure);
        void RemoveAdventure(Guid id);
        IEnumerable<Adventure> GetAdventuresForSpecialOffer(ClaimsPrincipal User);
        IEnumerable<AdventureImage> GetAdventureImages(Guid adventureId);
        void AddAdventureImage(Guid adventureId, CloudBlockBlob blockBlob);
        IEnumerable<AdventureDTO> GetInstructorAdventures(string userId);
        bool AdventureEditable(Guid adventureId);
        AdventureDTO GetAdventureDto(Guid adventureId);
        void Create(AdventureDTO dto);
        bool Subscribe(Guid adventureId, AdventureFavorites favorite, IdentityUser identityUser);
        IEnumerable<UserDetails> GetAllUserDetails();
        IEnumerable<AdventureRealisation> GetAdventureRealiastions(Guid id);
        bool AdventureExists(Guid id);
    }
}
