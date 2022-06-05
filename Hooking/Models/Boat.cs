using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class Boat : BaseModel
    {   
        [DisplayName("Naziv")]
        public string Name { get; set; }
        [DisplayName("Tip")]
        public string Type { get; set; }
        [DisplayName("Dužina")]
        public int Length { get; set; }
        [DisplayName("Kapacitet")]
        public int Capacity { get; set; }
        [DisplayName("Broj motora")]
        public string EngineNumber { get; set; }
        [DisplayName("Jačina motora")]
        public int EnginePower { get; set; }
        [DisplayName("Maksimalna brzina")]
        public int MaxSpeed { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Država")]
        public string Country { get; set; }
        public string CancelationPolicyId { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; } //300 characters max
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }
        [DisplayName("Cena po noćenju radnim danima")]
        public double RegularPrice { get; set; }
        [DisplayName("Cena po noćenju vikendima i praznicima")]
        public double WeekendPrice { get; set; }
        public string BoatOwnerId { get; set; }

        public bool? hasSubscribers { get; set; }

        public Boat() { }


    }
}
