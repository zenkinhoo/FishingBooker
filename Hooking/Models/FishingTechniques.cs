using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class FishingTechniques : BaseModel
    {
        public bool InstructorHasBoat { get; set; }
        public bool Inshore { get; set; }
        public bool Offshore { get; set; }
        
    }
}
