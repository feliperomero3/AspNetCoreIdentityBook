using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class RolesModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<IdentityRole> Roles => _roleManager.Roles.AsEnumerable();

        public IList<string> CurrentRoles { get; set; }
        public IList<string> AvailableRoles { get; set; }

        public async Task<int> GetUsersInRoleCountAsync(string roleName)
        {
            return (await _userManager.GetUsersInRoleAsync(roleName)).Count;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Edit Roles", Callback = "Roles" });
            }

            await SetPropertiesAsync();

            return Page();
        }

        public async Task<ActionResult> OnPostAddToListAsync(string role)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
            {
                return RedirectToPage();
            }

            result.ProcessOperationResult(ModelState);

            await SetPropertiesAsync();

            return Page();
        }

        public async Task<ActionResult> OnPostDeleteFromListAsync(string role)
        {
            var idRole = await _roleManager.FindByNameAsync(role);
            var result = await _roleManager.DeleteAsync(idRole);

            if (result.Succeeded)
            {
                return RedirectToPage();
            }

            result.ProcessOperationResult(ModelState);

            await SetPropertiesAsync();

            return Page();
        }

        public async Task<ActionResult> OnPostAddAsync([Required] string role)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Id);

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    var result = await _userManager.AddToRoleAsync(user, role);

                    if (result.Succeeded)
                    {
                        return RedirectToPage();
                    }

                    result.ProcessOperationResult(ModelState);

                    return Page();
                }
            }

            await SetPropertiesAsync();

            return Page();
        }

        public async Task<ActionResult> OnPostDeleteAsync(string role)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToPage();
        }

        private async Task SetPropertiesAsync()
        {
            var user = await _userManager.FindByIdAsync(Id);

            CurrentRoles = await _userManager.GetRolesAsync(user);

            AvailableRoles = _roleManager.Roles.Select(r => r.Name).Where(r => !CurrentRoles.Contains(r)).ToList();
        }
    }
}
