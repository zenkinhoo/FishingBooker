using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureReview : BaseModel
    {
        public string AdventureId { get; set; }
        public string UserDetailsId { get; set; }
        public DateTime CreationTime { get; } = DateTime.Now;
        public string Review { get; set; } // limited to 300 chars
        public string Grade { get; set; }
        public bool IsApproved { get; set; }
        
    }
}
