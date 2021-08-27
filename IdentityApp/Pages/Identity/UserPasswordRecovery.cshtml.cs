using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class UserPasswordRecoveryModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public UserPasswordRecoveryModel(UserManager<IdentityUser> userManager, IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync([Required] string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    await _emailService.SendPasswordRecoveryEmail(user, "UserPasswordRecoveryConfirm");
                }

                TempData["message"] = "We have sent you an email.\r\nClick the link it contains to choose a new password.";

                return RedirectToPage();
            }

            return Page();
        }
    }
}
