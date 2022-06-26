using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class TwoFactorAuthenticationRequiredModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public TwoFactorAuthenticationRequiredModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> OnPost(string returnUrl)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var isTwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);

            if (isTwoFactorClientRemembered)
            {
                await _signInManager.ForgetTwoFactorClientAsync();
            }

            // Caution: Do not use the SignInManager<T>.SignOutAsync method to sign out of the application because
            // it will throw an exception, reporting there is no handler for the external scheme.
            await HttpContext.SignOutAsync();

            return LocalRedirect($"signin?returnUrl={returnUrl}");
        }
    }
}
