using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    [Authorize]
    public class UserPageModel : PageModel
    {
        // No methods or properties required
    }
}
