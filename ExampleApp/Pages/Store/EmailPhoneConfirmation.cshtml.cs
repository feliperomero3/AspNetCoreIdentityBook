using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class EmailPhoneConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public EmailPhoneConfirmationModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string DataType { get; set; }


        [BindProperty(SupportsGet = true)]
        public string DataValue { get; set; }

        public bool IsEmail => DataType.Equals("email");

        public AppUser AppUser { get; set; }


        public async Task<ActionResult> OnGetAsync(string id)
        {
            AppUser = await _userManager.FindByIdAsync(id);

            if (DataValue != null && DataValue.Contains(':'))
            {
                var values = DataValue.Split(":");

                return await Validate(values[0], values[1]);
            }
            return Page();
        }

        public async Task<ActionResult> OnPostAsync(string id, string token, string dataValue)
        {
            AppUser = await _userManager.FindByIdAsync(id);

            return await Validate(dataValue, token);
        }

        private async Task<ActionResult> Validate(string value, string token)
        {
            IdentityResult result;

            if (IsEmail)
            {
                result = await _userManager.ChangeEmailAsync(AppUser, value, token);
            }
            else
            {
                result = await _userManager.ChangePhoneNumberAsync(AppUser, value, token);
            }
            if (result.Succeeded)
            {
                return Redirect($"/users/edit/{AppUser.Id}");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}
