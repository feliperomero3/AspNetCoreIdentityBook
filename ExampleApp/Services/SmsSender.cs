using System.Diagnostics;
using ExampleApp.Identity;

namespace ExampleApp.Services
{
    public class SmsSender
    {
        public void SendMessage(AppUser user, params string[] body)
        {
            Debug.WriteLine("--- SMS begins ---");
            Debug.WriteLine($"To: {user.PhoneNumber}");

            foreach (var str in body)
            {
                Debug.WriteLine(str);
            }

            Debug.WriteLine("--- SMS ends ---");
        }
    }
}
