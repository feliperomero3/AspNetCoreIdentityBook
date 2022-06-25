using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class UserLockoutsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public UserLockoutsModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<AppUser> Users => _userManager.Users
            .OrderByDescending(u => _userManager.IsLockedOutAsync(u).Result)
            .ThenBy(u => u.UserName);

        public async Task<bool> IsUserLockedOut(AppUser user) =>
            await _userManager.IsLockedOutAsync(user);

        public async Task<string> GetLockoutStatus(AppUser user)
        {
            if (await _userManager.IsLockedOutAsync(user))
            {
                TimeSpan remaining = (await _userManager.GetLockoutEndDateAsync(user))
                    .GetValueOrDefault()
                    .Subtract(DateTimeOffset.Now);

                return $"Locked Out ({remaining.Minutes} minutes " + $"{remaining.Seconds} seconds remaining)";
            }
            return "(No Lockout)";
        }

        public async Task<IActionResult> OnPost(string id, int minutes)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddMinutes(minutes));

            return RedirectToPage();
        }
    }
}
