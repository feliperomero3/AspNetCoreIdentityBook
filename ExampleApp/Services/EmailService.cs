using System;
using ExampleApp.Identity;

namespace ExampleApp.Services
{
    public class EmailService
    {
        public void SendMessage(AppUser user, string subject, params string[] body)
        {
            Console.WriteLine("--- Email begins ---");
            Console.WriteLine($"To: {user.EmailAddress}");
            Console.WriteLine($"Subject: {subject}");

            foreach (var str in body)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("--- Email ends ---");
        }
    }
}
