using LibraryData;
using LibraryService.EmailConfiguration;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace LibraryService
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public async Task SendEmailAsync(string toName, string toEmailAddress,
                                         string subject, string message)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SenderEmail));
            email.To.Add(new MailboxAddress(toName, toEmailAddress));
            email.Subject = subject;

            var body = new BodyBuilder
            {
                HtmlBody = message
            };

            email.Body = body.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback =
                    (sender, certificate, certChainType, errors) => true;

                // Enabling access for less secure apps, 
                // which means the client/app doesn’t use OAuth 2.0
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port, false);

                // Only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(_emailConfiguration.SenderEmail, _emailConfiguration.Password);

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    } 
}
