using System.Linq;
using System.Threading.Tasks;
using ExampleApp.Identity;
using ExampleApp.Identity.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class RecoveryCodesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly UserStore _userStore;

        public RecoveryCodesModel(UserManager<AppUser> manager, UserStore userStore)
        {
            _userManager = manager;
            _userStore = userStore;
        }

        public AppUser AppUser { get; set; }

        public RecoveryCode[] Codes { get; set; }

        public int RemainingCodes { get; set; }

        public async Task OnGet(string id)
        {
            AppUser = await _userManager.FindByIdAsync(id);

            if (AppUser is not null)
            {
                Codes = (await _userStore.GetCodesAsync(AppUser)).ToArray();
                
                RemainingCodes = await _userManager.CountRecoveryCodesAsync(AppUser);
            }
        }

        public async Task<ActionResult> OnPostAsync(string id)
        {
            AppUser = await _userManager.FindByIdAsync(id);
            
            if (AppUser is not null)
            {
                await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(AppUser, 10);
            }

            return RedirectToPage();
        }
    }
}
