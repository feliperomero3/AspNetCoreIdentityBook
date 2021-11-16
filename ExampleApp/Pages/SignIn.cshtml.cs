using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Custom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleApp.Pages
{
    public class SignInModel : PageModel
    {
        public SelectList Users => new(UsersAndClaims.Users, User.Identity.Name);

        public string Username { get; set; }

        public int? Code { get; set; }

        public void OnGet(int? code)
        {
            Code = code;
            Username = User.Identity.Name ?? "(No Signed In User)";
        }

        public async Task<ActionResult> OnPost([Required] string username)
        {
            var claim = new Claim(ClaimTypes.Name, username);
            var identity = new ClaimsIdentity(ExampleAppConstants.Scheme);

            identity.AddClaim(claim);

            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            return Redirect("/signin");
        }
    }
}
