using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    public class UserPasswordRecoveryConfirmModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenUrlEncoderService _tokenUrlEncoder;

        public UserPasswordRecoveryConfirmModel(UserManager<IdentityUser> userManager, TokenUrlEncoderService tokenUrlEncoder)
        {
            _userManager = userManager;
            _tokenUrlEncoder = tokenUrlEncoder;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
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
                    TempData["message"] = "Password changed.";

                    return RedirectToPage();
                }

                result.ProcessOperationResult(ModelState);
            }

            return Page();
        }
    }
}
