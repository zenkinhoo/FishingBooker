using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureFavorites : BaseModel
    {
        public string UserDetailsId { get; set; }
        public string AdventureId { get; set; }
    }
}
