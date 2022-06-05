using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hooking.Models;
using Hooking.Models.DTO;

namespace Hooking.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hooking.Models.BoatOwner> BoatOwner { get; set; }
        public DbSet<Hooking.Models.UserDetails> UserDetails { get; set; }
        public DbSet<Hooking.Models.Instructor> Instructor { get; set; }
        public DbSet<Hooking.Models.CancelationPolicy> CancelationPolicy { get; set; }
        public DbSet<Hooking.Models.Cottage> Cottage { get; set; }
        public DbSet<Hooking.Models.CottageOwnerReview> CottageOwnerReview { get; set; }
        public DbSet<Hooking.Models.CottageReservation> CottageReservation { get; set; }
        public DbSet<Hooking.Models.CottageReservationReview> CottageReservationReview { get; set; }
        public DbSet<Hooking.Models.CottageReview> CottageReview { get; set; }
        public DbSet<Hooking.Models.CottageRoom> CottageRoom { get; set; }
        public DbSet<Hooking.Models.CottageSpecialOffer> CottageSpecialOffer { get; set; }
        public DbSet<Hooking.Models.CottagesRooms> CottagesRooms { get; set; }
        public DbSet<Hooking.Models.Facilities> Facilities { get; set; }
        public DbSet<Hooking.Models.CottagesFacilities> CottagesFacilities { get; set; }
        public DbSet<Hooking.Models.CottagesHouseRules> CottagesHouseRules { get; set; }
        public DbSet<Hooking.Models.Adventure> Adventure { get; set; }
        public DbSet<Hooking.Models.AdventureFavorites> AdventureFavorites { get; set; }
        public DbSet<Hooking.Models.AdventureFishingEquipment> AdventureFishingEquipment { get; set; }
        public DbSet<Hooking.Models.AdventureFishingTechniques> AdventureFishingTechniques { get; set; }
        public DbSet<Hooking.Models.AdventureRealisation> AdventureRealisation { get; set; }
        public DbSet<Hooking.Models.AdventureReservation> AdventureReservation { get; set; }
        public DbSet<Hooking.Models.AdventureReservationReview> AdventureReservationReview { get; set; }
        public DbSet<Hooking.Models.AdventureReview> AdventureReview { get; set; }
        public DbSet<Hooking.Models.AdventureRules> AdventureRules { get; set; }
        public DbSet<Hooking.Models.AdventuresAdventureRules> AdventuresAdventureRules { get; set; }
        public DbSet<Hooking.Models.AdventureSpecialOffer> AdventureSpecialOffer { get; set; }
        public DbSet<Hooking.Models.Amenities> Amenities { get; set; }
        public DbSet<Hooking.Models.Boat> Boat { get; set; }
        public DbSet<Hooking.Models.BoatAmenities> BoatAmenities { get; set; }
        public DbSet<Hooking.Models.BoatFavorites> BoatFavorites { get; set; }
        public DbSet<Hooking.Models.BoatFishingEquipment> BoatFishingEquipment { get; set; }
        public DbSet<Hooking.Models.BoatOwnerReview> BoatOwnerReview { get; set; }
        public DbSet<Hooking.Models.BoatReservation> BoatReservation { get; set; }
        public DbSet<Hooking.Models.BoatReservationReview> BoatReservationReview { get; set; }
        public DbSet<Hooking.Models.BoatReview> BoatReview { get; set; }
        public DbSet<Hooking.Models.BoatRules> BoatRules { get; set; }
        public DbSet<Hooking.Models.BoatsBoatRules> BoatsBoatRules { get; set; }
        public DbSet<Hooking.Models.BoatSpecialOffer> BoatSpecialOffer { get; set; }
        public DbSet<Hooking.Models.FishingEquipment> FishingEquipment { get; set; }
        public DbSet<Hooking.Models.FishingTechniques> FishingTechniques { get; set; }
        public DbSet<Hooking.Models.UserDeleteRequest> UserDeleteRequest { get; set; }
        public DbSet<Hooking.Models.InstructorReview> InstructorReview { get; set; }
        public DbSet<Hooking.Models.CottageOwner> CottageOwner { get; set; }
        public DbSet<Hooking.Models.PrivilegedUserRequest> PrivilegedUserRequest { get; set; }
        public DbSet<Hooking.Models.CottageSpecialOfferReservation> CottageSpecialOfferReservation { get; set; }
        public DbSet<Hooking.Models.BoatSpecialOfferReservation> BoatSpecialOfferReservation { get; set; }
        public DbSet<Hooking.Models.HouseRules> HouseRules { get; set; }
        public DbSet<Hooking.Models.RegistrationRequest> RegistrationRequest { get; set; }
        public DbSet<Hooking.Models.FirstLoginAdmins> FirstLoginAdmins { get; set; }
        public DbSet<Hooking.Models.SystemOptions> SystemOptions { get; set; } 
        public DbSet<Hooking.Models.CottageFavorites> CottageFavorites { get; set; }
        public DbSet<Hooking.Models.CottageImage> CottageImages { get; set; }
        public DbSet<Hooking.Models.CottageNotAvailablePeriod> CottageNotAvailablePeriod { get; set; }
        public DbSet<Hooking.Models.CottageAppeal> CottageAppeal { get; set; }
        public DbSet<Hooking.Models.BoatAppeal> BoatAppeal { get; set; }
        public DbSet<Hooking.Models.AdventureAppeal> AdventureAppeal { get; set; }
        public DbSet<Hooking.Models.InstructorNotAvailablePeriod> InstructorNotAvailablePeriod { get; set; }
        public DbSet<Hooking.Models.AdventureImage> AdventureImage { get; set; }
        public DbSet<Hooking.Models.BoatImage> BoatImage { get; set; }
        public DbSet<Hooking.Models.BoatNotAvailablePeriod> BoatNotAvailablePeriod { get; set; }
        public DbSet<Hooking.Models.ReservationFilter> ReservationFilter { get; set; }
        public DbSet<Hooking.Models.BoatReservationFilter> BoatReservationFilter { get; set; }
        public DbSet<Hooking.Models.AdventureReservationFilter> AdventureReservationFilter { get; set; }
        public DbSet<Hooking.Models.FilteredCottages> FilteredCottages { get; set; }
        public DbSet<Hooking.Models.FilteredBoats> FilteredBoats { get; set; }
        public DbSet<Hooking.Models.FilteredInstructors> FilteredInstructors { get; set; }
        public DbSet<Hooking.Models.DTO.CottageReservationData> CottageReservationData { get; set; }
        
    }
}
