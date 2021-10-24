using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages
{
    [Authorize]
    public class JSClientModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
