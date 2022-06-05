using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Hooking.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class AwaitsApprovalModel : PageModel
    {

        public AwaitsApprovalModel()
        {
        }

        

        public IActionResult OnGetAsync()
        {
            return Page();
        }
    }
}
