using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class LockoutsModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public LockoutsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<IdentityUser> LockedOutUsers { get; set; }
        public IEnumerable<IdentityUser> OtherUsers { get; set; }

        public async Task<TimeSpan> LockoutTimeLeft(IdentityUser user) =>
            (await _userManager.GetLockoutEndDateAsync(user)).GetValueOrDefault().Subtract(DateTimeOffset.Now);

        public void OnGet()
        {
            LockedOutUsers = _userManager.Users
                .Where(user => user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
                .OrderBy(user => user.Email).ToList();

            OtherUsers = _userManager.Users.Where(user => !user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTimeOffset.Now)
                .OrderBy(user => user.Email).ToList();
        }

        public async Task<ActionResult> OnPostLockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.SetLockoutEnabledAsync(user, true);

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(5));

            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostUnlockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToPage();
        }
    }
}
