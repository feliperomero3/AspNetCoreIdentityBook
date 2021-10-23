using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    public class UserAccountCompleteModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenUrlEncoderService _tokenUrlEncoder;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserAccountCompleteModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TokenUrlEncoderService tokenUrlEncoder)
        {
            _userManager = userManager;
            _tokenUrlEncoder = tokenUrlEncoder;
            _signInManager = signInManager;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string email = null, string token = null)
        {
            Email = email;
            Token = token;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                var decodedToken = _tokenUrlEncoder.DecodeToken(Token);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, Password);

                if (result.Succeeded)
                {
                    return RedirectToPage("SignIn", new { });
                }

                result.ProcessOperationResult(ModelState);
            }
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return Page();
        }
    }
}
