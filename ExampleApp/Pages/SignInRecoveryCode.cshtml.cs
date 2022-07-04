using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Pages
{
    public class SignInRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<SignInRecoveryCodeModel> _logger;

        public SignInRecoveryCodeModel(SignInManager<AppUser> signInManager, ILogger<SignInRecoveryCodeModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<ActionResult> OnPost(string code, string returnUrl = "/")
        {
            if (string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError(string.Empty, "Verification code is required.");
            }
            else
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);

                if (result.Succeeded)
                {
                    var message = "User {UserName} signed in using a recovery code.";

                    _logger.LogInformation(message, user.UserName);

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
