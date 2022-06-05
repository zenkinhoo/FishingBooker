using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageReservation : BaseModel
    {
        public string CottageId { get; set; }
        public string UserDetailsId { get; set; }
        [DisplayName("Datum početka rezervacije")]
        public DateTime StartDate { get; set; }
        [DisplayName("Datum završetka rezervacije rezervacije")]
        public DateTime EndDate { get; set; }
        [DisplayName("Cena")]
        public double Price { get; set; }
        [DisplayName("Broj gostiju")]
        public int MaxPersonCount { get; set; }
        public bool IsReviewed { get; set; }
    }
}
