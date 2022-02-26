using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleApp.Pages
{
    public class SignInModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SignInModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public SelectList Users => new(_userManager.Users.OrderBy(u => u.EmailAddress), "EmailAddress", "EmailAddress");

        public string Username { get; set; }

        public int? Code { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(int? code, string returnUrl = null)
        {
            Code = code;
            ReturnUrl = returnUrl ?? Url.Content("~/");
            Username = User.Identity.Name ?? "(No Signed In User)";
        }

        public async Task<ActionResult> OnPost([Required] string username, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(username);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
        }
    }
}
