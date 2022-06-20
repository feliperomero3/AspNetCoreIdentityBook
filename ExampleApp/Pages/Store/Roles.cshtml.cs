using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class RolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesModel(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public AppRole[] Roles => _roleManager.Roles.OrderBy(r => r.Name).ToArray();

        public async Task<AppUser[]> GetUsersInRole(AppRole role) =>
            (await _userManager.GetUsersInRoleAsync(role.Name)).ToArray();

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostCreate(AppRole newRole)
        {
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                ProcessErrors(result.Errors);

                return Page();
            }

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostUpdate(AppRole editedRole)
        {
            var result = await _roleManager.UpdateAsync(editedRole);

            if (!result.Succeeded)
            {
                ProcessErrors(result.Errors);

                return Page();
            }

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    ProcessErrors(result.Errors);

                    return Page();
                }
            }

            return RedirectToPage();
        }

        private void ProcessErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError("", err.Description);
            }
        }
    }
}
