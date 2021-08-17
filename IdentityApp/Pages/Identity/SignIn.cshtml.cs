using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    public class SignInModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignInModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [Required]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: true, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return Redirect(ReturnUrl ?? "/");
                }
                else if (result.IsLockedOut)
                {
                    TempData["message"] = "Account Locked";
                }
                else if (result.IsNotAllowed)
                {
                    TempData["message"] = "Sign In Not Allowed";
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
                }
                else
                {
                    TempData["message"] = "Sign In Failed";
                }
            }

            return Page();
        }
    }
}
