using System.Threading.Tasks;
using ExampleApp.Identity;
using ExampleApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages.Store
{
    public class EmailPhoneChangeModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailService _emailService;
        private readonly SmsSender _smsSender;

        public EmailPhoneChangeModel(UserManager<AppUser> userManager, EmailService emailService, SmsSender smsSender)
        {
            _userManager = userManager;
            _emailService = emailService;
            _smsSender = smsSender;
        }

        [BindProperty(SupportsGet = true)]
        public string DataType { get; set; }

        public bool IsEmail => DataType.Equals("email");

        public AppUser AppUser { get; set; }

        public string LabelText => DataType == "email" ? "Email Address" : "Phone Number";

        public string CurrentValue => IsEmail ? AppUser.EmailAddress : AppUser.PhoneNumber;

        public async Task OnGet(string id, string dataValue)
        {
            AppUser = await _userManager.FindByIdAsync(id);
        }

        public async Task<ActionResult> OnPostAsync(string id, string dataValue)
        {
            AppUser = await _userManager.FindByIdAsync(id);

            if (IsEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(AppUser, dataValue);

                _emailService.SendMessage(AppUser, "Confirm Email", "Please click the link to confirm your email address:",
                    $"https://localhost:44324/validate/{id}/email/{dataValue}:{token}");
            }
            else
            {
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(AppUser, dataValue);

                _smsSender.SendMessage(AppUser, $"Your confirmation token is {token}");
            }

            return RedirectToPage("EmailPhoneConfirmation", new { id = id, dataType = DataType, dataValue = dataValue });
        }
    }
}
