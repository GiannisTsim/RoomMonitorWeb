using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace RoomMonitor.Services
{
    public class EmailSender
    {
        private readonly string _sendGridApiKey;

        public EmailSender(IConfiguration configuration)
        {
            _sendGridApiKey = configuration.GetSection("SENDGRID_API_KEY").Value;
        }

        //public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        private Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("invitation@test.com", "RoomMonitor"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }

        public async Task SendInvitationMail(string email, string callbackUrl)
        {
            string subject = "RoomMonitor Invitation";

            string body = @"<h4>You have been invited to join RoomMonitor!</h4>
                            <p>To get started, please <a href=""" + callbackUrl + @""">activate</a> your account by setting your password.</p>
                            <p>The account must be activated within 24 hours from receving this mail.</p>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
