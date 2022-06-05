using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class Amenities : BaseModel
    {
        [DisplayName("GPS")]
        public bool Gps { get; set; }
        [DisplayName("Radar")]
        public bool Radar { get; set; }
        [DisplayName("VHR Radio")]
        public bool VhrRadio { get; set; }
        [DisplayName("Sonar")]
        public bool Sonar { get; set; }
        [DisplayName("Fish-finder")]
        public bool FishFinder { get; set; }
        [DisplayName("WiFi")]
        public bool WiFi { get; set; }
    }
}
