using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class Instructor : BaseModel
    {
        public string UserDetailsId { get; set; }
        public double AverageGrade { get; set; }
        public int GradeCount { get; set; }
        public string Biography { get; set; } // limited to 200 characters
    }
}
