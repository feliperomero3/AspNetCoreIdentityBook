using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleApp.Pages.Store
{
    public class UserRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserRolesModel(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        public string NewRole { get; set; }

        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        public SelectList AvailableRoles { get; set; }

        public async Task OnGet()
        {
            var user = await GetUser();

            if (user != null)
            {
                Roles = (await _userManager.GetClaimsAsync(user))?
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                var roleClaims = (await _userManager.GetClaimsAsync(user))?
                    .Where(c => c.Type == ClaimTypes.Role);

                AvailableRoles = new SelectList(_roleManager.Roles
                    .OrderBy(r => r.Name)
                    .Where(r => !roleClaims.Any(c => c.Value == r.Name)).ToList(), "Name", "Name");
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
