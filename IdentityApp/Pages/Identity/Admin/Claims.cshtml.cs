using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class ClaimsModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Manage Claims", Callback = "Claims" });
            }

            var user = await _userManager.FindByIdAsync(Id);

            Claims = await _userManager.GetClaimsAsync(user);

            return Page();
        }

        public async Task<ActionResult> OnPostAsync([Required] string task, [Required] string type, [Required] string value, string oldValue)
        {
            var user = await _userManager.FindByIdAsync(Id);
            Claims = await _userManager.GetClaimsAsync(user);

            if (ModelState.IsValid)
            {
                var claim = new Claim(type, value);
                var result = IdentityResult.Success;

                switch (task)
                {
                    case "add":
                        result = await _userManager.AddClaimAsync(user, claim);
                        break;
                    case "change":
                        result = await _userManager.ReplaceClaimAsync(user,
                            new Claim(type, oldValue), claim);
                        break;
                    case "delete":
                        result = await _userManager.RemoveClaimAsync(user, claim);
                        break;
                }
                if (result.Succeeded)
                {
                    return RedirectToPage();
                }

                result.ProcessOperationResult(ModelState);
            }
            return Page();
        }
    }
}
