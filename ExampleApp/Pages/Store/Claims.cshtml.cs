using System;
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
    public class ClaimsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public ClaimsModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public AppUser AppUserObject { get; set; } = new AppUser();

        public IList<Claim> Claims { get; set; } = new List<Claim>();

        public string GetName(string claimType)
        {
            return (Uri.IsWellFormedUriString(claimType, UriKind.Absolute)
                ? System.IO.Path.GetFileName(new Uri(claimType).LocalPath)
                : claimType).ToUpper();
        }

        public async Task OnGet(string id)
        {
            if (id != null)
            {
                AppUserObject = await _userManager.FindByIdAsync(id) ?? new AppUser();

                Claims = (await _userManager.GetClaimsAsync(AppUserObject))
                    .OrderBy(c => c.Type)
                    .ThenBy(c => c.Value)
                    .ToList();
            }
        }

        public async Task<ActionResult> OnPostAdd(string id, string type, string value)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.AddClaimAsync(user, new Claim(type, value));

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostEdit(string id, string oldType, string type, string oldValue, string value)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var claim = new Claim(oldType, oldValue);
                var newClaim = new Claim(type, value);

                await _userManager.ReplaceClaimAsync(user, claim, newClaim);
            }
            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostDelete(string id, string type, string value)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.RemoveClaimAsync(user, new Claim(type, value));

            return RedirectToPage();
        }
    }
}
