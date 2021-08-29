using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpConfirmModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SignUpConfirmModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        public bool ShowConfirmedMessage { get; set; } = false;

        public async Task<ActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Token))
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Token));

                    var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                    if (result.Succeeded)
                    {
                        ShowConfirmedMessage = true;
                    }
                }
            }

            return Page();
        }
    }
}
