using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Pages
{
    public class ExternalAccountConfirmModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ExternalAccountConfirmModel> _logger;

        public ExternalAccountConfirmModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ExternalAccountConfirmModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public AppUser AppUser { get; set; } = new();

        public string ProviderDisplayName { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                _logger.LogWarning("No external login info found.");

                return Redirect(ReturnUrl);
            }
            else
            {
                AppUser.EmailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);
                AppUser.UserName = info.Principal.FindFirstValue(ClaimTypes.Name);
                ProviderDisplayName = info.ProviderDisplayName;

                return Page();
            }
        }

        public async Task<ActionResult> OnPostAsync(string username)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is not null)
            {
                AppUser.UserName = username;
                AppUser.EmailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);

                // I trust that the email address has been confirmed by the external authentication provider
                // so I set the IsEmailAddressConfirmed property to true before storing the AppUser object.
                AppUser.IsEmailAddressConfirmed = true;

                var result = await _userManager.CreateAsync(AppUser);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User account created from external {LoginProvider} provider.", info.LoginProvider);

                    await _userManager.AddClaimAsync(AppUser, new Claim(ClaimTypes.Role, "User"));

                    result = await _userManager.AddLoginAsync(AppUser, info);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Login added for {UserName}.", AppUser.UserName);

                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

                        return Redirect(ReturnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No external login found.");
            }
            return Page();
        }
    }
}
