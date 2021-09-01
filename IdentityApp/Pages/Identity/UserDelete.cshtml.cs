using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    public class UserDeleteModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _siginManager;

        public UserDeleteModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _siginManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _siginManager.SignOutAsync();

                return Challenge();
            }

            return Page();
        }
    }
}
