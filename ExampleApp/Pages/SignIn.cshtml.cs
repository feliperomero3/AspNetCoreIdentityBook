using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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

        public IEnumerable<AuthenticationScheme> GetExternalAuthenticationSchemesAsync() =>
            _signInManager.GetExternalAuthenticationSchemesAsync().GetAwaiter().GetResult();

        public string Username { get; set; }

        public int? Code { get; set; }

        public string ReturnUrl { get; set; }

        public string Message { get; set; }

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
                    _logger.LogInformation("User {username} signed in.", username);

                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {username} locked out.", username);

                    TimeSpan remaining = (await _userManager.GetLockoutEndDateAsync(user))
                        .GetValueOrDefault()
                        .Subtract(DateTimeOffset.Now);

                    Message = $"Locked Out ({remaining.Minutes} minutes {remaining.Seconds} seconds remaining)";

                    return Page();
                }
                else if (result.RequiresTwoFactor)
                {
                    _logger.LogWarning("User {username} requires Two-Factor sign in.", username);

                    return RedirectToPage("SignInTwoFactor", new { returnUrl = returnUrl });
                }
                else if (result.IsNotAllowed)
                {
                    Message = "Sign In Not Allowed";

                    return Page();
                }
            }

            Message = "Access Denied";

            return Page();
        }
    }
}
