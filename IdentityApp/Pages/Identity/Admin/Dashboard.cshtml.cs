using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DashboardModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly string[] emails = { "alice@example.com", "bob@example.com", "charlie@example.com" };

        public DashboardModel(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public int UsersCount { get; set; }
        public int UsersUnconfirmed { get; set; }
        public int UsersLockedout { get; set; }
        public int UsersTwoFactor { get; set; }

        public string DashboardRole => _configuration["Dashboard:Role"] ?? "Dashboard";

        public void OnGet()
        {
            UsersCount = _userManager.Users.Count();
            UsersUnconfirmed = _userManager.Users.Where(u => !u.EmailConfirmed).Count();
            UsersLockedout = _userManager.Users.Where(u => u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now).Count();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (var existingUser in _userManager.Users.ToList())
            {
                if (emails.Contains(existingUser.Email) || !await _userManager.IsInRoleAsync(existingUser, DashboardRole))
                {
                    var result = await _userManager.DeleteAsync(existingUser);

                    result.ProcessOperationResult(ModelState);
                }
            }

            foreach (var email in emails)
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

            UsersCount = _userManager.Users.Count();

            return Page();
        }
    }
}
