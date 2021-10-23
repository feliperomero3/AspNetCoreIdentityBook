using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignInModel : UserPageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SignInModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Required]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: true, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return LocalRedirect(ReturnUrl ?? "/");
                }
                else if (result.IsLockedOut)
                {
                    TempData["message"] = "Account Locked.";
                }
                else if (result.IsNotAllowed)
                {
                    TempData["message"] = "Sign In Not Allowed.";

                    var user = await _userManager.FindByEmailAsync(Email);

                    if (user != null)
                    {
                        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                        if (!isEmailConfirmed)
                        {
                            return RedirectToPage("SignUpConfirm");
                        }
                    }
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
                }
                else
                {
                    TempData["message"] = "Sign In Failed.";
                }
            }

            return Page();
        }

        public ActionResult OnPostExternal(string provider)
        {
            string callbackUrl = Url.Page("SignIn", "Callback", new { ReturnUrl });
            var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }

        public async Task<IActionResult> OnGetCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            if (result.Succeeded)
            {
                return Redirect(WebUtility.UrlDecode(ReturnUrl ?? "/"));
            }
            else if (result.IsLockedOut)
            {
                TempData["message"] = "Account Locked.";
            }
            else if (result.IsNotAllowed)
            {
                TempData["message"] = "Sign In Not Allowed.";
            }
            else
            {
                TempData["message"] = "Sign In Failed.";
            }

            return RedirectToPage();
        }
    }
}
