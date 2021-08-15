using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class ViewModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ViewModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IdentityUser IdentityUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<string> PropertyNames => typeof(IdentityUser).GetProperties().Select(p => p.Name);

        public string GetPropertyValue(string name) => typeof(IdentityUser).GetProperty(name).GetValue(IdentityUser)?.ToString();

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "View User", Callback = "View" });
            }
            IdentityUser = await _userManager.FindByIdAsync(Id);
            return Page();
        }
    }
}
