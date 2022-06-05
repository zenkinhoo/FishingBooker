using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatSpecialOfferReservation : BaseModel
    {
        public string BoatSpecialOfferId { get; set; }
        public string UserDetailsId { get; set; }
    }
}
