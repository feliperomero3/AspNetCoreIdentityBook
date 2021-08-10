using System.Net.Mail;
using System.Threading.Tasks;
using IdentityApp.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace IdentityApp.Services
{
    public class EmailSender : IEmailSender
    {
        public readonly SmtpClient _smtpClient;
        private readonly IOptions<SmtpSettings> _options;

        public EmailSender(SmtpClient smtpClient, IOptions<SmtpSettings> options)
        {
            _smtpClient = smtpClient;
            _options = options;
            _smtpClient.Host = options.Value.Hostname;
            _smtpClient.Port = options.Value.Port;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailAddress from = new MailAddress(_options.Value.From);
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(message);
        }
    }
}
