using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<IActionResult> OnPost(AppUser user)
        {
            IdentityResult result;

            var storeUser = await _userManager.FindByIdAsync(user.Id);

            if (storeUser == null)
            {
                result = await _userManager.CreateAsync(user);
            }
            else
            {
                storeUser.UpdateFrom(user);

                result = await _userManager.UpdateAsync(storeUser);
            }
            if (result.Succeeded)
            {
                return RedirectToPage("./Users", new { searchname = user.Id });
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
