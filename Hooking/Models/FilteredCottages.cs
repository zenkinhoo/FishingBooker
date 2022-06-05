using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class FilteredCottages: BaseModel
    {

        public string CottageId { get; set; }

        [DisplayName("Naziv")]
        public string Name { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Cena")]
        public double Price { get; set; }
        [DisplayName("Broj soba")]
        public int RoomCount { get; set; }
        [DisplayName("Površina")]
        public int Area { get; set; }
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }

        public FilteredCottages() { }

        public FilteredCottages(string cottageId, string name, string address, string city, int roomCount, int area, double averageGrade, int gradeCount)
        {
            CottageId = cottageId;
            Name = name;
            Address = address;
            City = city;
            RoomCount = roomCount;
            Area = area;
            AverageGrade = averageGrade;
            GradeCount = gradeCount;
        }
    }
}
