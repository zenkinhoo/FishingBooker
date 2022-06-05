using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureReservationFilter: BaseModel
    {
        [DisplayName("Datum pocetka rezervacije")]
        public DateTime startDate { get; set; }

        [DisplayName("Datum zavrsetka rezervacije")]
        public DateTime endDate { get; set; }

        [DisplayName("Maksimalna cena")]
        public double? price { get; set; }

        [DisplayName("Minimalna prosecna ocena")]
        public double? AverageGrade { get; set; }

        [DisplayName("Maksimalni broj osoba")]
        public int MaxPersonCount { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }

        public AdventureReservationFilter() { }
    }
}
