using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class FilteredInstructors: BaseModel
    {

        public string instructorId { get; set; }
        [DisplayName("Ime")]
        public string FirstName { get; set; }
        [DisplayName("Prezime")]
        public string LastName { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }

        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Broj ocena")]
        public int GradeCount { get; set; }

        public FilteredInstructors() { }

        public FilteredInstructors(string firstName, string lastName, string address, string city, double averageGrade, int gradeCount)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            City = city;
            AverageGrade = averageGrade;
            GradeCount = gradeCount;
        }
    }
}
