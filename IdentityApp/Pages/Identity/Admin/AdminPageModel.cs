using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class AdminPageModel : PageModel
    {
        public void ProcessIdentityOperationResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors ?? Enumerable.Empty<IdentityError>())
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
