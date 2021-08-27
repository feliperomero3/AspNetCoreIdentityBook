using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class EditModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IdentityUser IdentityUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public class EditBindingTarget
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }
        }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Edit User", Callback = "Edit" });
            }
            IdentityUser = await _userManager.FindByIdAsync(Id);
            return Page();
        }

        public async Task<ActionResult> OnPostAsync([FromForm(Name = "IdentityUser")] EditBindingTarget userData)
        {
            if (!string.IsNullOrEmpty(Id) && ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Id);

                if (user != null)
                {
                    user.UserName = userData.Email;
                    user.Email = userData.Email;
                    user.EmailConfirmed = true;

                    if (!string.IsNullOrEmpty(userData.PhoneNumber))
                    {
                        user.PhoneNumber = userData.PhoneNumber;
                    }

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToPage();
                    }

                    result.ProcessOperationResult(ModelState);
                }
            }

            return Page();
        }
    }
}
