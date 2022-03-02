using ExampleApp.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ExampleApp.Pages
{
    public class SignInModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<SignInModel> _logger;

        public SignInModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<SignInModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public SelectList Users => new(_userManager.Users.OrderBy(u => u.EmailAddress), "EmailAddress", "EmailAddress");

        public string Username { get; set; }

        public int? Code { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(int? code, string returnUrl = null)
        {
            Code = code;
            ReturnUrl = returnUrl ?? Url.Content("~/");
            Username = User.Identity.Name ?? "(No Signed In User)";
        }

        public async Task<ActionResult> OnPost([Required] string username, [Required] string password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            SignInResult result;

            var user = await _userManager.FindByEmailAsync(username);

            if (user != null && !string.IsNullOrEmpty(password))
            {
                result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {username} logged in.", username);
                    return LocalRedirect(returnUrl);
                }
            }

            Code = StatusCodes.Status401Unauthorized;
            return Page();
        }
    }
}
