using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; }

        public IEnumerable<AppUser> Users { get; set; } = Enumerable.Empty<AppUser>();

        public async Task OnGet()
        {
            if (_userManager.SupportsQueryableUsers)
            {
                var normalizedName = _userManager.NormalizeName(SearchName ?? string.Empty);

                Users = string.IsNullOrEmpty(SearchName)
                    ? _userManager.Users.OrderBy(u => u.UserName)
                    : _userManager.Users.Where(u => u.Id == SearchName || u.NormalizedUserName.Contains(normalizedName)).OrderBy(u => u.UserName);
            }
            if (SearchName != null)
            {
                var nameUser = await _userManager.FindByNameAsync(SearchName);

                if (nameUser != null)
                {
                    Users = Users.Append(nameUser);
                }

                var idUser = await _userManager.FindByIdAsync(SearchName);

                if (idUser != null)
                {
                    Users = Users.Append(idUser);
                }
            }
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToPage();
        }
    }
}
