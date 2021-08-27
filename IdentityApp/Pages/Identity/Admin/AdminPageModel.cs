using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Pages.Identity.Admin
{
    public class AdminPageModel : UserPageModel
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
