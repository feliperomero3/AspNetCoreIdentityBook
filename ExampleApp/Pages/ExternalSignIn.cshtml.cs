using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class ExternalSignInModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalSignInModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string providerName, string returnUrl = "/")
        {
            /* There two levels of redirection to be handled when preparing for external authentication.
             * The standard signin process captures the URL that the user requested that led to the challenge response.
             * This value is added as a query string parameter of the URL to which the authentication handler should redirect the
             * browser after the user has been authenticated */
            var redirectUrl = Url.Page("/ExternalSignIn", "Correlate", new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

            return new ChallengeResult(providerName, properties);
        }
    }
}
