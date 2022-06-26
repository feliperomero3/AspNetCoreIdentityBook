using System.Threading.Tasks;
using ExampleApp.Identity;
using ExampleApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ExampleApp.Pages
{
    public class SignInTwoFactorModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly SmsSender _smsSender;

        public SignInTwoFactorModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, SmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _smsSender = smsSender;
        }

        public bool IsAuthenticatorEnabled { get; set; }

        public async Task OnGet()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user is not null)
            {
                IsAuthenticatorEnabled = user.IsAuthenticatorEnabled;

                if (!IsAuthenticatorEnabled)
                {
                    // The user's security token is updated to invalidate any previous tokens.
                    await _userManager.UpdateSecurityStampAsync(user);

                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, IdentityConstants.TwoFactorUserIdScheme);

                    _smsSender.SendMessage(user, $"Your security code is {token}.");
                }
            }
        }

        public async Task<ActionResult> OnPost(string code, string rememberMe, [FromQuery] string returnUrl)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user is not null)
            {
                var scheme = IdentityConstants.TwoFactorUserIdScheme;

                SignInResult result;

                IsAuthenticatorEnabled = user.IsAuthenticatorEnabled;

                if (IsAuthenticatorEnabled)
                {
                    var authenticatorCode = code.Replace(" ", string.Empty);

                    result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, isPersistent: false, rememberClient: !string.IsNullOrEmpty(rememberMe));
                }
                else
                {
                    result = await _signInManager.TwoFactorSignInAsync(scheme, code, isPersistent: true, rememberClient: !string.IsNullOrEmpty(rememberMe));
                }

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl ?? "/");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Not allowed.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Authentication failed.");
                }
            }
            return Page();
        }
    }
}
