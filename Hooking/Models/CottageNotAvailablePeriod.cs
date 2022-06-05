using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageNotAvailablePeriod : BaseModel
    {
        public string CottageId { get; set; }
        [DisplayName("Od")]
        public DateTime StartTime { get; set; }
        [DisplayName("Do")]
        public DateTime EndTime { get; set; }

    }
}
