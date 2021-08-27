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
            UsersCount = _userManager.Users.Count();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (IdentityUser existingUser in _userManager.Users.ToList())
            {
                var result = await _userManager.DeleteAsync(existingUser);

                result.ProcessOperationResult(ModelState);
            }
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
                    result = await _userManager.AddPasswordAsync(user, "mysecret");
                }

                result.ProcessOperationResult(ModelState);
            }

            return Page();
        }
    }
}
