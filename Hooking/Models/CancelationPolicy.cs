using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;



namespace Hooking.Models
{
    public class CancelationPolicy : BaseModel
    {
        [DisplayName("Besplatno otkazivanje do")]
        public int FreeUntil { get; set; }
        [DisplayName("Procenat od cene koji se čuva u slučaju otkaza")]
        public int PenaltyPercentage { get; set; }
    }
}
