using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class CottageRoom : BaseModel
    {
        [DisplayName("Broj kreveta")]
        public int BedCount { get; set; }
        [DisplayName("Klima uređaj")]
        public bool AirCondition { get; set; }
        [DisplayName("TV")]
        public bool TV { get; set; }
        [DisplayName("Terasa")]
        public bool Balcony { get; set; }
    }
}
