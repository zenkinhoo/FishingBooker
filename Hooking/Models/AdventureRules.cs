using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureRules : BaseModel
    {
        public bool ChildFriendly { get; set; }
        public bool YouKeepCatch { get; set; }
        public bool CatchAndReleaseAllowed { get; set; }
        public bool CabinSmoking { get; set; }
    }
}
