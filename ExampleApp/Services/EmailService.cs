using System.Diagnostics;
using ExampleApp.Identity;

namespace ExampleApp.Services
{
    public class EmailService
    {
        public void SendMessage(AppUser user, string subject, params string[] body)
        {
            Debug.WriteLine("--- Email begins ---");
            Debug.WriteLine($"To: {user.EmailAddress}");
            Debug.WriteLine($"Subject: {subject}");

            foreach (var str in body)
            {
                Debug.WriteLine(str);
            }

            Debug.WriteLine("--- Email ends ---");
        }
    }
}
