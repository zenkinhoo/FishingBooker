using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageSpecialOfferReservation : BaseModel
    {
        public string CottageSpecialOfferId { get; set; }
        public string UserDetailsId { get; set; }
    }
}
