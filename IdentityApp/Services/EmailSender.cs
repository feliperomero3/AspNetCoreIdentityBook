using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IdentityApp.Services
{
    public class EmailSender : IEmailSender
    {
        public readonly SmtpClient _smtpClient;

        public EmailSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailAddress from = new MailAddress("identityapp@localhost");
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            _smtpClient.Send(message);

            return Task.CompletedTask;
        }
    }
}
