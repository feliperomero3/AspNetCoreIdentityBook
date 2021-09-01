using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DeleteModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IdentityUser IdentityUser { get; set; }

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
