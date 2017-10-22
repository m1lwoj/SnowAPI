using EmailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SnowTests.Integration
{
    public class EmailSenderTests
    {
        [Fact]
        public void NotEmptyConfiguration()
        {
            ConfigurationProvider configuratioonProvider = new ConfigurationProvider();

            var result = configuratioonProvider.GetCredentials();

            Assert.NotEmpty(result.DisplayName);
            Assert.NotEmpty(result.EmailAddress);
            Assert.NotEmpty(result.Password);
            Assert.NotEmpty(result.SMTPhost);
            Assert.NotEqual(result.SMTPport, default(int));
        }

        [Fact]
        public void HtmlActivateContentNotEmpty()
        {
            HtmlContentProvider htmlContentProvider = new HtmlContentProvider("user@user.com");

            var result = htmlContentProvider.GetContentWithCode(ContentType.ActivateAccount, "1234");

            Assert.NotEmpty(result);
        }

        [Fact]
        public void HtmlResetPasswordContentNotEmpty()
        {
            HtmlContentProvider htmlContentProvider = new HtmlContentProvider("user@user.com");

            var result = htmlContentProvider.GetContentWithCode(ContentType.ResetPassword, "1234");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async void SendEmailSuccess()
        {
            Sender emailSender = new Sender();

            try
            {
                await emailSender.SendCodeEmail(ContentType.ActivateAccount, "m1l@g.pl", "1234");
            }
            catch (Exception ex)
            {
                Assert.Equal("Expected no exception" , ex.Message);
            }

        }
    }
}


