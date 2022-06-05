using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class FishingEquipment : BaseModel
    {
        [DisplayName("Živi mamci")]
        public bool LiveBite { get; set; }
        [DisplayName("Oprema za fly-fishing")]
        public bool FlyFishingGear { get; set; }
        [DisplayName("Mamci")]
        public bool Lures { get; set; }
        [DisplayName("Štapovi za pecanje")]
        public bool RodsReelsTackle { get; set; }


    }
}
