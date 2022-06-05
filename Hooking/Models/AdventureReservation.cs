using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureReservation : BaseModel
    {
        public string AdventureRealisationId { get; set; }
        public string UserDetailsId { get; set; }
        public bool IsReviewed { get; set; }
    }
}
