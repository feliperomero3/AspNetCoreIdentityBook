using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class ViewClaimsPrincipalModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ViewClaimsPrincipalModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Callback { get; set; }

        public ClaimsPrincipal Principal { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser",
                new
                {
                    Label = "View ClaimsPrincipal",
                    Callback = "ClaimsPrincipal"
                });
            }

            var user = await _userManager.FindByIdAsync(Id);

            Principal = await _signInManager.CreateUserPrincipalAsync(user);

            return Page();
        }
    }
}
