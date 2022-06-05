using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models
{
    public class SystemOptions : BaseModel
    {
        [Display(Name="Podešavanje")]
        public string OptionName { get; set; }

        [Display(Name = "Vrednost")]
        public string OptionValue { get; set; }
    }
}
