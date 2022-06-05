using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models.DTO
{
    public class DeleteRequestDTO
    {
        [Display(Name = "Odgovor")]
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public DeletionType Type { get; set; }
        public string UserDetailsId { get; set; }
    }
}
