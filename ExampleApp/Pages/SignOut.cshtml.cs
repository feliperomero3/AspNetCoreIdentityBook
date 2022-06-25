using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class SignOutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public SignOutModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public string Username { get; private set; }

        public void OnGet()
        {
            Username = User.Identity.Name ?? "(No Signed In User)";
        }

        public async Task<ActionResult> OnPost(string forgetMe)
        {
            if (!string.IsNullOrEmpty(forgetMe))
            {
                await _signInManager.ForgetTwoFactorClientAsync();
            }

            await HttpContext.SignOutAsync();

            return RedirectToPage("SignIn");
        }
    }
}
