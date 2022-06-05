using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatReservationReview : BaseModel
    {
        public string BoatReservationId { get; set; }
        [DisplayName("Recenzija")]
        public string Review { get; set; } // limited to 300 characters
        [DisplayName("Da li su se gosti pojavili?")]
        public bool DidntShow { get; set; }
        [DisplayName("Da li želite da prijavite gosta?")]
        public bool ReceivedPenalty { get; set; }
        public bool IsReviewedByAdmin { get; set; }
    }
}
