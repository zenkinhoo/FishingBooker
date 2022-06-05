using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public enum CreationType { INSTRUCTOR, BOATOWNER, COTTAGEOWNER }
    public class PrivilegedUserRequest : BaseModel
    {
        public string UserDetailsId { get; set; }
        public CreationType Type { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreationTime { get; } = DateTime.Now;
        public string Description { get; set; } //max 300 characters
    }
}
