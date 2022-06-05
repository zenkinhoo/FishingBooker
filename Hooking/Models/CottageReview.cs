using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageReview : BaseModel
    {
        public string CottageId { get; set; }
        public string UserDetailsId { get; set; }
        [DisplayName("Datum objave")]
        public DateTime CreationTime { get; } = DateTime.Now;
        [DisplayName("Recenzija")]
        public string Review { get; set; } // limited to 300 chars
        [DisplayName("Ocena")]
        public string Grade { get; set; }
        [DisplayName("Da li je odobrena recenzija?")]
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
