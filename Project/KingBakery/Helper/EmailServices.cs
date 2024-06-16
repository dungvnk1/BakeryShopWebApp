using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace KingBakery.Helper
{
    public class EmailServices
    {
        private readonly EmailSettings _emailSettings;

        public EmailServices(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.EnableSsl = _emailSettings.EnableSSl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Username, "King Bakery"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                client.Send(mailMessage);
            }
        }
    }
}
