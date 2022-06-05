using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class Cottage : BaseModel
    {
        [DisplayName("Naziv")]
        public string Name { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Država")]
        public string Country { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; } // limited ?
        [DisplayName("Broj soba")]
        public int RoomCount { get; set; }
        [DisplayName("Površina")]
        public int Area { get; set; }
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }
        public string CancelationPolicyId { get; set; }
        [DisplayName("Cena po noćenju radnim danima")]
        public double RegularPrice { get; set; }
        [DisplayName("Cena po noćenju vikendima i praznicima")]
        public double WeekendPrice { get; set; }
        public string CottageOwnerId { get; set; }

        public bool? hasSubscribers { get; set; }

        public Cottage()
        {

        }
        public Cottage(string Name, string Address, string City, string Country, string Description, int RoomCount, int Area, double RegularPrice, double WeekendPrice)
        {
            this.Name = Name;
            this.Address = Address;
            this.City = City;
            this.Country = Country;
            this.Description = Description;
            this.RoomCount = RoomCount;
            this.Area = Area;
            this.RegularPrice = RegularPrice;
            this.WeekendPrice = WeekendPrice;
        }

    }
}
