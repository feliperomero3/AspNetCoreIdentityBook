using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ExampleApp.Pages.Store
{
    public class EditUserModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public EditUserModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public AppUser AppUserObject { get; set; } = new AppUser();

        public async Task OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                AppUserObject = await _userManager.FindByIdAsync(id) ?? new AppUser();
            }
        }

        public async Task<IActionResult> OnPostAsync(AppUser user, string newPassword)
        {
            var result = IdentityResult.Success;

            var storeUser = await _userManager.FindByIdAsync(user.Id);

            if (storeUser == null)
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    ModelState.AddModelError(string.Empty, "Password Required.");
                    return Page();
                }
                result = await _userManager.CreateAsync(user, newPassword);
            }
            else
            {
                storeUser.UpdateFrom(user, out var IsChanged);

                if (newPassword != null)
                {
                    if (await _userManager.HasPasswordAsync(storeUser))
                    {
                        await _userManager.RemovePasswordAsync(storeUser);
                    }
                    result = await _userManager.AddPasswordAsync(storeUser, newPassword);
                }

                if (IsChanged && _userManager.SupportsUserSecurityStamp)
                {
                    await _userManager.UpdateSecurityStampAsync(storeUser);
                }
                if (result != null && result.Succeeded)
                {
                    result = await _userManager.UpdateAsync(storeUser);
                }
            }
            if (result.Succeeded)
            {
                return RedirectToPage("Users");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description ?? "Error");
                }

                AppUserObject = user;

                return Page();
            }
        }
    }
}
