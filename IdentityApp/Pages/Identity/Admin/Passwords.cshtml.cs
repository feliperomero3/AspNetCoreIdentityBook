using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class PasswordsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public PasswordsModel(UserManager<IdentityUser> userManager, IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public IdentityUser IdentityUser { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [Compare(nameof(Password))]
        public string Confirmation { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Password", Callback = "Passwords" });
            }

            IdentityUser = await _userManager.FindByIdAsync(Id);

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(IdentityUser);

            return Page();
        }

        public async Task<ActionResult> OnPostSetPasswordAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityUser = await _userManager.FindByIdAsync(Id);

                if (await _userManager.HasPasswordAsync(IdentityUser))
                {
                    await _userManager.RemovePasswordAsync(IdentityUser);
                }

                var result = await _userManager.AddPasswordAsync(IdentityUser, Password);

                if (result.Succeeded)
                {
                    TempData["message"] = "Password Changed.";

                    return RedirectToPage();
                }

                result.ProcessOperationResult(ModelState);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUserChangeAsync()
        {
            IdentityUser = await _userManager.FindByIdAsync(Id);

            await _userManager.RemovePasswordAsync(IdentityUser);
            await _emailService.SendPasswordRecoveryEmail(IdentityUser, "/Identity/UserPasswordRecoveryConfirm");

            TempData["message"] = "Email Sent to User.";

            return RedirectToPage();
        }
    }
}
