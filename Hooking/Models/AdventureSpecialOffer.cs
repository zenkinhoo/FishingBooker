using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureSpecialOffer : BaseModel
    {
        public string AdventureId { get; set; }
        public string UserDetailsId { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; } // max 300 char
        [DisplayName("Datum i vreme početka")]
        public DateTime StartDate { get; set; }
        [DisplayName("Trajanje")]
        public double Duration { get; set; }
        [DisplayName("Specijalna cena")]
        public double Price { get; set; }
        [DisplayName("Maksimalan broj gostiju")]
        public int MaxPersonCount { get; set; }
        public bool IsReserved { get; set; }
        [DisplayName("Specijalna ponuda važi od")]
        public DateTime ValidFrom { get; set; }
        [DisplayName("Specijalna ponuda važi do")]
        public DateTime ValidTo { get; set; }
    }
}
