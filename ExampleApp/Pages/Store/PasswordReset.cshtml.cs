using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ExampleApp.Identity;
using ExampleApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Pages.Store
{
    public class PasswordResetModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SmsSender _smsSender;
        private readonly ILogger<PasswordChangeModel> _logger;

        public PasswordResetModel(UserManager<AppUser> userManager, SmsSender smsSender, ILogger<PasswordChangeModel> logger)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _logger = logger;
        }

        public async Task<ActionResult> OnPostAsync([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.LogInformation("Password reset token generated for {email}", email);
                _smsSender.SendMessage(user, $"Your password reset token is {token}");
            }

            return RedirectToPage("PasswordResetConfirm", new { email = email });
        }
    }
}
