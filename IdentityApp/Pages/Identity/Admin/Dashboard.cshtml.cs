using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DashboardModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string[] emails = { "alice@example.com", "bob@example.com", "charlie@example.com" };

        public DashboardModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public int UsersCount { get; set; }
        public int UsersUnconfirmed { get; set; }
        public int UsersLockedout { get; set; }
        public int UsersTwoFactor { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (string email in emails)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToPage();
                }

                foreach (IdentityError error in result.Errors ?? Enumerable.Empty<IdentityError>())
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
