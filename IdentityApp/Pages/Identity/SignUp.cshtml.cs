using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public SignUpModel(UserManager<IdentityUser> userManager, IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                    if (!isEmailConfirmed)
                    {
                        /*
                         * A common problem arises when the user forgets to confirm the account and subsequently repeats the 
                         * signup process using the same email address. This will cause an error because the unconfirmed account 
                         * is already in the user store, essentially trapping the user. 
                         * To avoid this problem, I check the user store to see if it contains an unconfirmed account and perform a redirection if it does.
                         */
                        return RedirectToPage("SignUpConfirm");
                    }
                }

                user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, Password);

                    if (result.Succeeded)
                    {
                        await _emailService.SendAccountConfirmEmail(user, "SignUpConfirm");

                        return RedirectToPage("SignUpConfirm");
                    }
                    else
                    {
                        /*
                         * If a password doesn't pass validation, an error will be reported after the
                         * IdentityUser has been stored, which will produce an account that cannot be used 
                         * but which is associated with the user's email address. 
                         * The simplest way to solve this problem is to remove the IdentityUser object
                         * from the store if there is a problem with the password.
                         */
                        await _userManager.DeleteAsync(user);
                    }
                }

                result.ProcessOperationResult(ModelState);
            }

            return Page();
        }
    }
}
