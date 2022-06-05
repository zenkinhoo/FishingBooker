using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hooking.Areas.Identity.Pages.Account;

namespace Hooking.Models
{
    public class RegistrationRequest : BaseModel
    {
        public string UserDetailsId { get; set; }
        [Display(Name = "Uloga")]
        public RegistrationType Type { get; set; }
        [Display(Name = "Obrazloženje")]
        public string Description { get; set; }
    }
}
