using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class UserRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRolesModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        public async Task OnGet()
        {
            var user = await GetUser();
            if (user != null)
            {
                Roles = (await _userManager.GetClaimsAsync(user))?
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);
            }
        }

        public async Task<IActionResult> OnPostAdd(string newRole)
        {
            await _userManager.AddClaimAsync(await GetUser(), new Claim(ClaimTypes.Role, newRole));
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(string role)
        {
            await _userManager.RemoveFromRoleAsync(await GetUser(), role);
            return RedirectToPage();
        }

        private Task<AppUser> GetUser()
        {
            return Id == null ? null : _userManager.FindByIdAsync(Id);
        }
    }
}
