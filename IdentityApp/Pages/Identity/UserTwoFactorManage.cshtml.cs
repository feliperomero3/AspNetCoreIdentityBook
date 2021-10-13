using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    public class UserTwoFactorManageModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserTwoFactorManageModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IdentityUser IdentityUser { get; set; }

        public async Task<bool> IsTwoFactorEnabled() => await _userManager.GetTwoFactorEnabledAsync(IdentityUser);

        public async Task OnGetAsync()
        {
            IdentityUser = await _userManager.GetUserAsync(User);
        }

        public async Task<IActionResult> OnPostDisableAsync()
        {
            IdentityUser = await _userManager.GetUserAsync(User);

            var result = await _userManager.SetTwoFactorEnabledAsync(IdentityUser, false);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();

                return RedirectToPage("Index", new { });
            }

            result.ProcessOperationResult(ModelState);

            return Page();
        }

        public async Task<ActionResult> OnPostGenerateCodes()
        {
            IdentityUser = await _userManager.GetUserAsync(User);

            TempData["RecoveryCodes"] = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(IdentityUser, 10);

            return RedirectToPage("UserRecoveryCodes");
        }
    }
}
