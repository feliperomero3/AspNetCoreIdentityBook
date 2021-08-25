using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    public class UserPasswordChangeModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserPasswordChangeModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public class PasswordChangeBindingTarget
        {
            [Required]
            public string Current { get; set; }

            [Required]
            public string NewPassword { get; set; }

            [Required]
            [Compare(nameof(NewPassword))]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(PasswordChangeBindingTarget data)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, data.Current, data.NewPassword);

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
