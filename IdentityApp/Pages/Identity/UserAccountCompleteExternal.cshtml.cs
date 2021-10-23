using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class UserAccountCompleteExternalModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenUrlEncoderService _encoder;

        public UserAccountCompleteExternalModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TokenUrlEncoderService encoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _encoder = encoder;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public IdentityUser IdentityUser { get; set; }

        public async Task<string> GetExternalProvider() =>
            (await _userManager.GetLoginsAsync(IdentityUser)).FirstOrDefault()?.ProviderDisplayName;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            IdentityUser = await _userManager.FindByIdAsync(id);

            if ((id == null || IdentityUser == null) && !TempData.ContainsKey("errorMessage"))
            {
                return RedirectToPage("SignIn");
            }

            return Page();
        }

        public async Task<IActionResult> OnGetCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            string email = info?.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            IdentityUser = await _userManager.FindByEmailAsync(email);

            if (string.IsNullOrEmpty(email))
            {
                return Error("External service has not provided an email address.");
            }
            else if (IdentityUser == null)
            {
                return Error("Your email address doesn't match.");
            }

            var result = await _userManager.AddLoginAsync(IdentityUser, info);

            if (!result.Succeeded)
            {
                return Error("Cannot store external login.");
            }

            return RedirectToPage(new { id = IdentityUser.Id });
        }

        public async Task<IActionResult> OnPostAsync(string provider)
        {
            IdentityUser = await _userManager.FindByEmailAsync(Email);

            var decodedToken = _encoder.DecodeToken(Token);
            var resetTokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
            const string tokenPurpose = UserManager<IdentityUser>.ResetPasswordTokenPurpose;

            bool isValid = await _userManager.VerifyUserTokenAsync(IdentityUser, resetTokenProvider, tokenPurpose, decodedToken);

            if (!isValid)
            {
                return Error("Invalid token");
            }

            string callbackUrl = Url.Page("UserAccountCompleteExternal", "Callback", new { Email, Token });

            var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return new ChallengeResult(provider, props);
        }

        private ActionResult Error(string error)
        {
            TempData["errorMessage"] = error;
            return RedirectToPage();
        }
    }
}
