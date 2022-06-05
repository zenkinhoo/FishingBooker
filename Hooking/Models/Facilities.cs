using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class Facilities : BaseModel
    {
        [DisplayName("Parking")]
        public bool Parking { get; set; }
        [DisplayName("WiFi")]
        public bool Wifi { get; set; }
        [DisplayName("Grejanje")]
        public bool Heating { get; set; }
        [DisplayName("Roštilj")]
        public bool BarbecueFacilities { get; set; }
        [DisplayName("Online prijava u smeštaj")]
        public bool OnlineCheckin { get; set; }
        [DisplayName("Đakuzi")]
        public bool Jacuzzi { get; set; }
        [DisplayName("Pogled na more")]
        public bool SeaView { get; set; }
        [DisplayName("Pogled na planinu")]
        public bool MountainView { get; set; }
        [DisplayName("Kuhinja")]
        public bool Kitchen { get; set; }
        [DisplayName("Veš mašina")]
        public bool WashingMachine { get; set; }
        [DisplayName("Aerodromski transfer")]
        public bool AirportShuttle { get; set; }
        [DisplayName("Unutrašnji bazen")]
        public bool IndoorPool { get; set; }
        [DisplayName("Spoljašnji bazen")]
        public bool OutdoorPool { get; set; }
        [DisplayName("Mini-bar")]
        public bool StockedBar { get; set; }
        [DisplayName("Dvorište")]
        public bool Garden { get; set; }
    }
}
