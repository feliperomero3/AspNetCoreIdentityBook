using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class AuthenticatorSetupModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthenticatorSetupModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public AppUser AppUser { get; set; }

        public string AuthenticatorUrl { get; set; }

        public async Task OnGetAsync()
        {
            AppUser = await _userManager.FindByIdAsync(Id);

            if (AppUser is not null)
            {
                if (AppUser.AuthenticatorKey is not null)
                {
                    // Authenticators that can scan a QR code expect to receive a URL in the format
                    // otpauth://totp/<label>?secret=<key> where<label> identifies the user account and where<key> is the secret key.
                    // See https://github.com/google/google-authenticator/wiki/Key-Uri-Format
                    // for full details of the URL format used for authenticator QR codes.
                    AuthenticatorUrl = $"otpauth://totp/ExampleApp:{AppUser.EmailAddress}?secret={AppUser.AuthenticatorKey}";
                }
            }
        }
        public async Task<ActionResult> OnPostAsync(string task)
        {
            AppUser = await _userManager.FindByIdAsync(Id);
            if (AppUser is not null)
            {
                switch (task)
                {
                    case "enable":
                        AppUser.IsAuthenticatorEnabled = true;
                        AppUser.IsTwoFactorAuthenticationEnabled = true;
                        break;
                    case "disable":
                        AppUser.IsAuthenticatorEnabled = false;
                        AppUser.IsTwoFactorAuthenticationEnabled = false;
                        break;
                    default:
                        await _userManager.ResetAuthenticatorKeyAsync(AppUser);
                        break;
                }
                await _userManager.UpdateAsync(AppUser);
            }
            return RedirectToPage();
        }
    }
}
