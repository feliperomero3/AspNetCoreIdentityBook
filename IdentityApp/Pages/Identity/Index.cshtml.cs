using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Pages.Identity
{
    public class IndexModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }
        public string Phone { get; set; }

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            Email = currentUser.Email ?? "(No value)";
            Phone = currentUser.PhoneNumber ?? "(No value)";
        }
    }
}
