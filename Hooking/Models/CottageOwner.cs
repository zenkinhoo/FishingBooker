using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageOwner : BaseModel
    {
        public string UserDetailsId { get; set; }
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }

    }
}
