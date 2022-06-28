using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class SignInRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public SignInRecoveryCodeModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ActionResult> OnPost(string code, string returnUrl = "/")
        {
            if (string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError(string.Empty, "Verification code is required.");
            }
            else
            {
                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                }
            }

            return Page();
        }
    }
}
