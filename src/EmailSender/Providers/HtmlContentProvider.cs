using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace EmailSender
{
    public class HtmlContentProvider
    {
        private const string userEmail = "$USEREMAIL$";
        private const string generatedCode = "$GENERATEDCODE$";
        private const string unwantedMessage = "$UNWANTEDMESSAGE$";
        private const string mainMessage = "$MAINMESSAGE$";

        private const string resetPasswordFileName = "resetpassword.html";
        private const string confirmAccountFileName = "confirmaccount.html";

        private string _email;

        public HtmlContentProvider(string email)
        {
            _email = email;
        }

        public string GetContentWithCode(ContentType type, string code)
        {
            string filename = string.Empty;
            string mainMessageContent = string.Empty;
            switch (type)
            {
                case ContentType.ResetPassword:
                    filename = resetPasswordFileName;
                    mainMessageContent = "In order to reset your password use code below.";
                    break;
                case ContentType.ActivateAccount:
                    filename = confirmAccountFileName;
                    mainMessageContent = "In order to confirm your account use code below.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string htmlContent = StaticFileReader.GetFileText($"EmailSender.templates.{filename}");

            htmlContent = htmlContent.Replace(userEmail, _email);
            htmlContent = htmlContent.Replace(generatedCode, code);
            htmlContent = htmlContent.Replace(unwantedMessage, "If this message is unwanted, please let us know contact@snowapp.com");
            htmlContent = htmlContent.Replace(mainMessage, mainMessageContent);

            return htmlContent;
        }
    }
}
