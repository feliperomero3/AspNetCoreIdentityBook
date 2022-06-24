using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class RoleClaimsModel : PageModel
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleClaimsModel(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public AppRole Role { get; private set; }

        public IEnumerable<Claim> Claims => Role.Claims ?? new[] { new Claim(string.Empty, string.Empty) };

        public async Task OnGet(string id)
        {
            Role = await _roleManager.FindByIdAsync(id);
        }

        public async Task<ActionResult> OnPostAdd(string id, string type, string value)
        {
            Role = await _roleManager.FindByIdAsync(id);

            await _roleManager.AddClaimAsync(Role, new Claim(type, value));

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostEdit(string id, string type, string value, string oldType, string oldValue)
        {
            Role = await _roleManager.FindByIdAsync(id);

            await _roleManager.RemoveClaimAsync(Role, new Claim(oldType, oldValue));
            await _roleManager.AddClaimAsync(Role, new Claim(type, value));

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostDelete(string id, string type, string value)
        {
            Role = await _roleManager.FindByIdAsync(id);

            await _roleManager.RemoveClaimAsync(Role, new Claim(type, value));

            return RedirectToPage();
        }
    }
}
