using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity
{
    public class UserRecoveryCodesModel : UserPageModel
    {
        [TempData]
        public string[] RecoveryCodes { get; set; }

        public ActionResult OnGet()
        {
            if (RecoveryCodes == null || RecoveryCodes.Length == 0)
            {
                return RedirectToPage("UserTwoFactorManage");
            }

            return Page();
        }
    }
}
