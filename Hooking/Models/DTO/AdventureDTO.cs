using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Hooking.Models.DTO
{
    public class AdventureDTO
    {
        public string AdventureId { get; set; }
        public string RowVersion { get; set; }
        public string InstructorId { get; set; }
        [Display(Name="Instruktor")]
        public string InstructorName { get; set; }
        [Display(Name = "Biografija instruktora")]
        public string InstructorBiography { get; set; }
        [Display(Name = "Naziv avanture")]
        public string Name { get; set; }
        [Display(Name = "Adresa")]
        public string Address { get; set; }
        public string FullAddress { get; set; }
        [Display(Name = "Grad")]
        public string City { get; set; }
        [Display(Name = "Država")]
        public string Country { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Molimo unesite pozitivan broj")]
        [Display(Name = "Maksimalan broj osoba")]
        public int MaxPersonCount { get; set; }
        public string CancellationPolicyId { get; set; }
        [Display(Name = "Otkazivanje")]
        public string CancelationPolicyDescription { get; set; }
        [Display(Name = "Prosečna ocena")]
        public string AverageGrade { get; set; }
        [Display(Name = "Dostupna oprema")]
        public string FishingEquipment { get; set; }
        [Display(Name="Pravila")]
        public string Rules { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Molimo unesite pozitivan broj")]
        [Display(Name = "Cena")]
        public int Price { get; set; }
        [Display(Name= "Za decu")]
        public bool ChildFriendly { get; set; }
        [Display(Name = "Zadržavanje ulova")]
        public bool YouKeepCatch { get; set; }
        [Display(Name = "Uhvati-baci")]
        public bool CatchAndReleaseAllowed { get; set; }
        [Display(Name = "Pušenje u kabini")] 
        public bool CabinSmoking { get; set; }
        public AdventureDTO(){}

        public AdventureDTO(Adventure adventure)
        {
            AdventureId = adventure.Id.ToString();
            RowVersion = adventure.RowVersion == null ? "" : adventure.RowVersion.ToString();
            InstructorId = adventure.InstructorId;
            Name = adventure.Name;
            Address = adventure.Address;
            City = adventure.City;
            Country = adventure.Country;
            FullAddress = $"{Address},{City},{Country}";
            Description = adventure.Description;
            MaxPersonCount = adventure.MaxPersonCount;
            CancellationPolicyId = adventure.CancellationPolicyId;
            AverageGrade = adventure.AverageGrade.ToString();
            Price = adventure.Price;
        }

        public void PopulateFieldsFromCancellationPolicy(CancelationPolicy policy)
        {
            CancelationPolicyDescription = $"Besplatno {policy.FreeUntil} dana ranije, kazna: {policy.PenaltyPercentage}%";
        }

        public void PopulateFieldsFromFishingEquipment(FishingEquipment equipment)
        {
            string LiveBite = equipment.LiveBite ? "Da" : "Ne";
            string Lures = equipment.Lures ? "Da" : "Ne";
            string FlyFishingGear = equipment.FlyFishingGear ? "Da" : "Ne";
            string RodsReelsTackle = equipment.RodsReelsTackle ? "Da" : "Ne";
            FishingEquipment =
                $"Live bite: {LiveBite}\nLures: {Lures}\nFly fishing gear: {FlyFishingGear}\nRods reels tackle: {RodsReelsTackle}";
        }

        public void PopulateFieldsFromRulesWithFishing(AdventureRules rules)
        {
            string ChildFriendly = rules.ChildFriendly ? "Da" : "Ne";
            string CabinSmoking = rules.CabinSmoking ? "Da" : "Ne";
            string CatchAndReleaseAllowed = rules.CatchAndReleaseAllowed ? "Da" : "Ne";
            string YouKeepCatch = rules.YouKeepCatch ? "Da" : "Ne";
            Rules = $"Za decu: {ChildFriendly} | Pušenje u kabini: {CabinSmoking} | Zadržavanje ulova: {YouKeepCatch} | Uhvati-baci: {CatchAndReleaseAllowed}";
        }

        public void PopulateFieldsFromRulesWithoutFishing(AdventureRules rules)
        {
            string ChildFriendly = rules.ChildFriendly ? "Da" : "Ne";
            Rules = $"Za decu: {ChildFriendly}";
        }

    }
}
