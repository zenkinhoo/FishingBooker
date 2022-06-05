using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatNotAvailablePeriod : BaseModel
    {
        public string BoatId { get; set; }
        [DisplayName("Od")]
        public DateTime StartTime { get; set; }
        [DisplayName("Do")]
        public DateTime EndTime { get; set; }
    }
}
