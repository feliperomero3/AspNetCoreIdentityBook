using System;
using ExampleApp.Identity;

namespace ExampleApp.Services
{
    public class SmsSender
    {
        public void SendMessage(AppUser user, params string[] body)
        {
            Console.WriteLine("--- SMS begins ---");
            Console.WriteLine($"To: {user.PhoneNumber}");

            foreach (var str in body)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("--- SMS ends ---");
        }
    }
}
