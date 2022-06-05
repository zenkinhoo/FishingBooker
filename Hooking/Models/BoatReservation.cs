using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatReservation : BaseModel
    {
        public string BoatId { get; set; }
        public string UserDetailsId { get; set; }
        [DisplayName("Datum početka rezervacije")]
        public DateTime StartDate { get; set; }
        [DisplayName("Datum završetka rezervacije")]
        public DateTime EndDate { get; set; }
        [DisplayName("Cena")]
        public double Price { get; set; }
        [DisplayName("Broj gostiju")]
        public int PersonCount { get; set; }
        public bool IsReviewed { get; set; }
     
    }
}
