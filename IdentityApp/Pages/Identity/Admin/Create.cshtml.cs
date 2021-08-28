using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public CreateModel(UserManager<IdentityUser> userManager, IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        [EmailAddress]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await _emailService.SendPasswordRecoveryEmail(user, "/Identity/UserAccountComplete");

                    TempData["message"] = "Account Created.";

                    return RedirectToPage();
                }

                result.ProcessOperationResult(ModelState);
            }

            return Page();
        }
    }
}
