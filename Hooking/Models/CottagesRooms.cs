using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottagesRooms : BaseModel
    {
        public string CottageId { get; set; }
        public string CottageRoomId { get; set; }
    }
}
