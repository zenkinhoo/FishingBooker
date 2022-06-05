using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatSpecialOffer : BaseModel
    {
        public string BoatId { get; set; }
        [DisplayName("Datum početka rezervacije")]
        public DateTime StartDate { get; set; }
        [DisplayName("Datum završetka rezervacije")]
        public DateTime EndDate { get; set; }
        [DisplayName("Specijalna cena")]
        public double Price { get; set; }
        [DisplayName("Maksimalan broj putnika")]
        public int MaxPersonCount { get; set; }
        [DisplayName("Opis specijalne ponude")]
        public string Description { get; set; } // max 300 char
        public bool IsReserved { get; set; }
        [DisplayName("Specijalna ponuda važi od")]
        public DateTime ValidFrom { get; set; }
        [DisplayName("Specijalna ponuda važi do")]
        public DateTime ValidTo { get; set; }
    }
}
