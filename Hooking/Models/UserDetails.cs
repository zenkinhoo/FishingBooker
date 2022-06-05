using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hooking.Models
{
    public class UserDetails : BaseModel
    { 
        public string IdentityUserId { get; set; }
        [DisplayName("Ime")]
        public string FirstName { get; set; }
        [DisplayName("Prezime")]
        public string LastName { get; set; }
        [DisplayName("Adresa")]
        public string Address { get; set; }
        [DisplayName("Grad")]
        public string City { get; set; }
        [DisplayName("Država")]
        public string Country { get; set; }
        [DisplayName("Broj kaznenih poena")]
        public int PenaltyCount { get; set; }
        [DisplayName("Odobren")]
        public bool Approved { get; set; }
    }
}
