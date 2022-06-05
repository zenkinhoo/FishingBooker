using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public enum DeletionType
    {
        [Display(Name = "Običan korisnik")]
        USER,
        [Display(Name = "Instruktor")]
        INSTRUCTOR,
        [Display(Name = "Vlasnik broda")]
        BOATOWNER,
        [Display(Name = "Vlasnik vikendice")]
        COTTAGEOWNER,
        [Display(Name = "Administrator")]
        ADMIN
    }
    public class UserDeleteRequest : BaseModel
    {
        public string UserDetailsId { get; set; }
        [Display(Name = "Vreme podnošenja")]
        public string Description { get; set; }
        [Display(Name = "Odobren")]
        public bool IsApproved { get; set; }
        [Display(Name = "Tip korisnika")]
        public DeletionType Type { get; set; } 
        public bool isReviewed { get; set; }

    }
}
