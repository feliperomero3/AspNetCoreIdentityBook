using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin
{
    public class SelectUserModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SelectUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<IdentityUser> Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Label { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Callback { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; }

        public void OnGet()
        {
            Users = _userManager.Users
                .Where(u => string.IsNullOrWhiteSpace(Filter) || u.Email.Contains(Filter))
                .OrderBy(u => u.Email)
                .AsEnumerable();
        }

        public ActionResult OnPost()
        {
            return RedirectToPage(new { Callback, Filter });
        }
    }
}
