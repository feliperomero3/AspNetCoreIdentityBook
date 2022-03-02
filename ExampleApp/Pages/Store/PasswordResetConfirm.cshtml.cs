using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExampleApp.Pages.Store
{
    public class PasswordResetConfirmModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PasswordChangeModel> _logger;

        public PasswordResetConfirmModel(UserManager<AppUser> userManager, ILogger<PasswordChangeModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsEmailChanged { get; set; } = false;

        public async Task<ActionResult> OnPostAsync(string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                if (result.Succeeded)
                {
                    return RedirectToPage(new { IsEmailChanged = true });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Password Change Error");
            }

            return Page();
        }
    }
}
