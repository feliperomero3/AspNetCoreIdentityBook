using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Pages.Store
{
    public class PasswordChangeModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PasswordChangeModel> _logger;

        public PasswordChangeModel(UserManager<AppUser> userManager, ILogger<PasswordChangeModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public bool Success { get; set; } = false;

        public async Task<ActionResult> OnPostAsync([Required] string oldPassword, [Required] string newPassword)
        {
            var username = HttpContext.User.Identity.Name;

            if (username != null)
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user != null && !string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
                {
                    var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User password changed.");
                        Success = true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return Page();
        }
    }
}
