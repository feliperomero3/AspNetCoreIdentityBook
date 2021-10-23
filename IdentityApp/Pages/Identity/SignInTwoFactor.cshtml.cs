using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignInTwoFactorModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignInTwoFactorModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public string ReturnUrl { get; set; }

        [BindProperty]
        [Required]
        public string Token { get; set; }

        [Required]
        public bool RememberMe { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user != null)
            {
                string token = Regex.Replace(Token, @"\s", "");
                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(token, true, RememberMe);

                if (!result.Succeeded)
                {
                    result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(token);
                }

                if (result.Succeeded)
                {
                    if (await _userManager.CountRecoveryCodesAsync(user) <= 3)
                    {
                        return RedirectToPage("SignInCodesWarning");
                    }

                    return LocalRedirect(WebUtility.UrlDecode(ReturnUrl) ?? "/");
                }
            }

            ModelState.AddModelError("Token", "Invalid token or recovery code.");

            return Page();
        }
    }
}
