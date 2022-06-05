using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class BoatImage
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string BoatId { get; set; }
    }
}
