using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Pages
{
    public class ExternalSignInModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ExternalSignInModel> _logger;

        public ExternalSignInModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<ExternalSignInModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public string ProviderDisplayName { get; set; }

        public ActionResult OnPost(string providerName, string returnUrl = "/")
        {
            /* There are two levels of redirection to be handled when preparing for external authentication.
             * The standard signin process captures the URL that the user requested that led to the challenge response.
             * This value is added as a query string parameter of the URL to which the authentication handler should redirect the
             * browser after the user has been authenticated */
            var redirectUrl = Url.Page("/ExternalSignIn", "Correlate", new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

            return new ChallengeResult(providerName, properties);
        }

        public async Task<ActionResult> OnGetCorrelate(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user is null)
            {
                var externalEmail = info.Principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

                user = await _userManager.FindByEmailAsync(externalEmail);

                if (user is null)
                {
                    var message = "User with external email {externalEmail} was not found in the local user store.";
                    
                    _logger.LogInformation(message, externalEmail);

                    return RedirectToPage("/ExternalAccountConfirm", new { returnUrl });
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);

            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} with Provider {LoginProvider} signed in.", user.UserName, info.LoginProvider);

                return RedirectToPage("ExternalSignIn", "Confirm", new { info.ProviderDisplayName, returnUrl });
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("User {UserName} with {LoginProvider} provider requires Two-Factor sign in.", user.UserName, info.LoginProvider);

                var postSignInUrl = Url.Page("/ExternalSignIn", "Confirm", new { info.ProviderDisplayName, returnUrl });

                return RedirectToPage("/SignInTwoFactor", new { ReturnUrl = postSignInUrl });
            }

            return RedirectToPage(new { error = true, returnUrl });
        }

        public async Task OnGetConfirm()
        {
            var provider = User.FindFirstValue(ClaimTypes.AuthenticationMethod);
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

            ProviderDisplayName = schemes.FirstOrDefault(s => s.Name == provider)?.DisplayName ?? provider;
        }
    }
}
