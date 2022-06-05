using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureReservationReview : BaseModel
    {
        public string AdventureReservationId { get; set; }
        [DisplayName("Recenzija")]
        public string Review { get; set; } // limited to 300 characters
        [DisplayName("Da li se gost pojavio?")]
        public bool DidntShow { get; set; }
        [DisplayName("Da li želite da prijavite gosta?")]
        public bool ReceivedPenalty { get; set; }
        public bool IsReviewedByAdmin { get; set; }
    }
}
