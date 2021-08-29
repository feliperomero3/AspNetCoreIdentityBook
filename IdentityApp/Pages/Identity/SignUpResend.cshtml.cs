using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpResendModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public SignUpResendModel(UserManager<IdentityUser> userManager, IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [EmailAddress]
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                    if (!isEmailConfirmed)
                    {
                        await _emailService.SendAccountConfirmEmail(user, "SignUpConfirm");
                    }
                }

                /*
                 * This is displayed regardless of whether the email is sent. This is to prevent this page 
                 * from being used to determine which accounts exist and if they are awaiting confirmation.
                 */
                TempData["message"] = "Confirmation email sent. Check your inbox.";

                return RedirectToPage(new { Email });
            }

            return Page();
        }
    }
}
