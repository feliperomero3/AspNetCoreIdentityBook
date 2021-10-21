using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpExternalModel : UserPageModel
    {
        public SignUpExternalModel(UserManager<IdentityUser> usrMgr, SignInManager<IdentityUser> signMgr)
        {
            SignInManager = signMgr;
            UserManager = usrMgr;
        }

        public SignInManager<IdentityUser> SignInManager { get; }
        public UserManager<IdentityUser> UserManager { get; }

        public IdentityUser IdentityUser { get; set; }

        public async Task<string> GetExternalProvider() =>
            (await UserManager.GetLoginsAsync(IdentityUser)).FirstOrDefault()?.ProviderDisplayName;

        public async Task<ActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("SignUp");
            }
            else
            {
                IdentityUser = await UserManager.FindByIdAsync(id);
                if (IdentityUser == null)
                {
                    return RedirectToPage("SignUp");
                }
            }
            return Page();
        }

        public async Task<ActionResult> OnGetCallbackAsync()
        {
            var info = await SignInManager.GetExternalLoginInfoAsync();
            var email = info?.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Error("External service has not provided an email address.");
            }
            else if ((await UserManager.FindByEmailAsync(email)) != null)
            {
                return Error("An account already exists with your email address.");
            }
            var identUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var result = await UserManager.CreateAsync(identUser);
            if (result.Succeeded)
            {
                identUser = await UserManager.FindByEmailAsync(email);
                result = await UserManager.AddLoginAsync(identUser, info);

                if (result.Succeeded)
                {
                    return RedirectToPage(new { id = identUser.Id });
                }
            }
            return Error("An account could not be created.");
        }

        public ActionResult OnPost(string provider)
        {
            var callbackUrl = Url.Page("SignUpExternal", "Callback");
            var props = SignInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }

        private ActionResult Error(string error)
        {
            TempData["errorMessage"] = error;
            return RedirectToPage();
        }
    }
}
