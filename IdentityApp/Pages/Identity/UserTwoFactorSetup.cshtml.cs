using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    public class UserTwoFactorSetupModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserTwoFactorSetupModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IdentityUser IdentityUser { get; set; }

        public string AuthenticatorKey { get; set; }

        public string QrCodeUrl { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            await LoadAuthenticatorKeys();

            if (await _userManager.GetTwoFactorEnabledAsync(IdentityUser))
            {
                return RedirectToPage("UserTwoFactorManage");
            }

            return Page();
        }

        public async Task<ActionResult> OnPostConfirmAsync([Required] string confirm)
        {
            await LoadAuthenticatorKeys();

            if (ModelState.IsValid)
            {
                var token = Regex.Replace(confirm, @"\s", "");
                var tokenProvider = _userManager.Options.Tokens.AuthenticatorTokenProvider;

                var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(IdentityUser, tokenProvider, token);

                if (isCodeValid)
                {
                    TempData["RecoveryCodes"] = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(IdentityUser, 10);

                    await _userManager.SetTwoFactorEnabledAsync(IdentityUser, true);
                    await _signInManager.RefreshSignInAsync(IdentityUser);

                    return RedirectToPage("UserRecoveryCodes");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Confirmation code invalid");
                }
            }

            return Page();
        }

        private async Task LoadAuthenticatorKeys()
        {
            IdentityUser = await _userManager.GetUserAsync(User);
            AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(IdentityUser);

            if (AuthenticatorKey == null)
            {
                await _userManager.ResetAuthenticatorKeyAsync(IdentityUser);

                AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(IdentityUser);

                await _signInManager.RefreshSignInAsync(IdentityUser);
            }

            QrCodeUrl = $"otpauth://totp/ExampleApp:{IdentityUser.Email}?secret={AuthenticatorKey}";
        }
    }
}
