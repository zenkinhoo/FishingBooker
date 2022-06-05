using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class AdventureRealisation : BaseModel
    {
        public string AdventureId { get; set; }
        [Display(Name="Trajanje")]
        public double Duration { get; set; }
        [Display(Name = "Cena")]
        public double Price { get; set; }
        [Display(Name = "Datum početka")]
        public DateTime StartDate { get; set; }
        

    }
}
