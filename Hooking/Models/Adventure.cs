using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Models.DTO;

namespace Hooking.Models
{
    public class Adventure : BaseModel
    {
        public string InstructorId { get; set; }
        [DisplayName("Naziv")]
        public string Name { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Država")]
        public string Country { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Molimo unesite pozitivan broj")]
        [DisplayName("Maksimalan broj gostiju")]
        public int MaxPersonCount { get; set; }
        [DisplayName("Uslovi otkaza")]
        public string CancellationPolicyId { get; set; }
        [DisplayName("Prosečna ocena")]
        public double AverageGrade { get; set; }
        [DisplayName("Cena")]
        public int Price { get; set; }

        public bool? hasSubscribers { get; set; }


        public Adventure(){}
        public Adventure(AdventureDTO dto)
        {
            InstructorId = dto.InstructorId;
            Name = dto.Name;
            Address = dto.Address;
            City = dto.City;
            Country = dto.Country;
            Description = dto.Description;
            MaxPersonCount = dto.MaxPersonCount;
            CancellationPolicyId = dto.CancellationPolicyId;
            if (dto.AverageGrade == null)
            {
                AverageGrade = 0;
            }
            else
            {
                AverageGrade = double.Parse(dto.AverageGrade);
            }
            Price = dto.Price;
        }
    }
}
