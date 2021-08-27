using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignOutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignOutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage();
        }
    }
}
