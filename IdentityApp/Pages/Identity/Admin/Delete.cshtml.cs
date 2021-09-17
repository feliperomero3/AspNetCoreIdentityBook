using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DeleteModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public DeleteModel(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public IdentityUser IdentityUser { get; set; }

        public string DashboardUser => _configuration["Dashboard:User"] ?? "admin@example.com";

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Delete", Callback = "Delete" });
            }

            IdentityUser = await _userManager.FindByIdAsync(Id);

            return Page();
        }

        public async Task<ActionResult> OnPostAsync()
        {
            IdentityUser = await _userManager.FindByIdAsync(Id);

            var result = await _userManager.DeleteAsync(IdentityUser);

            if (result.Succeeded)
            {
                return RedirectToPage("Dashboard");
            }

            return Page();
        }
    }
}
