using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EmailSender
{
    public class Sender : ISender
    {
        /// <summary>
        /// Sends email containing code.
        /// </summary>
        /// <param name="type">Code email content type</param>
        /// <param name="receiverEmail">Receiver email</param>
        /// <param name="code">Code</param>
        public async Task SendCodeEmail(ContentType type, string receiverEmail, string code)
        {
            var content = new HtmlContentProvider(receiverEmail).GetContentWithCode(type, code);
            string emailTitle = type == ContentType.ActivateAccount ? "SnowApp - account activation" : "SnowApp - reset password";

            await SendEmailAsync(receiverEmail, emailTitle, content);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var configuration = new ConfigurationProvider().GetCredentials();

            var emailMessage = new MimeMessage();

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;

            emailMessage.From.Add(new MailboxAddress(configuration.DisplayName, configuration.EmailAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                await client.ConnectAsync(configuration.SMTPhost, configuration.SMTPport, false).ConfigureAwait(false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(configuration.EmailAddress, configuration.Password);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        //WTF IS THAT http://stackoverflow.com/questions/777607/the-remote-certificate-is-invalid-according-to-the-validation-procedure-using ?
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
