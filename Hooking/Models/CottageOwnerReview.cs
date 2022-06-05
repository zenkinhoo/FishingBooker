using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageOwnerReview : BaseModel
    {
        public string CottageOwnerId { get; set; }
        public string UserDetailsId { get; set; }
        [DisplayName("Datum objave")]
        public DateTime CreationDate { get; } = DateTime.Now;
        [DisplayName("Recenzija")]
        public string Review { get; set; } // limited to 300 characters
        [DisplayName("Ocena")]
        public int Grade { get; set; }
        [DisplayName("Da li je odobrena recenzija?")]
        public bool IsApproved { get; set; }
    }
}
