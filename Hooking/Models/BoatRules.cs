using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatRules : BaseModel
    {
        [DisplayName("Prilagođeno deci")]
        public bool ChildFriendly { get; set; }
        [DisplayName("Dozvoljeno čuvanje ulova")]
        public bool YouKeepCatch { get; set; }
        [DisplayName("Dozvoljeno pecanje i puštanje ulova")]
        public bool CatchAndReleaseAllowed { get; set; }
        [DisplayName("Zabranjeno pušenje u kabinama")]
        public bool CabinSmoking { get; set; }
    }
}
