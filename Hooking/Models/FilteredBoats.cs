using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class FilteredBoats: BaseModel
    {
        public string BoatId { get; set; }

        [DisplayName("Naziv")]
        public string Name { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Cena")]
        public double Price { get; set; }
        [DisplayName("Dužina")]
        public int Length { get; set; }

        [DisplayName("Maksimalna brzina")]
        public int MaxSpeed { get; set; }
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }
    }
}
