using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class FeaturesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public FeaturesModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<(string, string)> Features { get; set; }

        public void OnGet()
        {
            Features = _userManager.GetType().GetProperties()
                .Where(prop => prop.Name.StartsWith("Supports"))
                .OrderBy(p => p.Name)
                .Select(prop => (prop.Name, prop.GetValue(_userManager).ToString()));
        }
    }
}
