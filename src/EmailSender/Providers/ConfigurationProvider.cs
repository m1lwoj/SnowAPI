using Newtonsoft.Json;

namespace EmailSender
{
    public class ConfigurationProvider
    {
        private const string credentialsFileName = "credentials.json";

        public EmailConfigurationModel GetCredentials()
        {
            string fileContent = StaticFileReader.GetFileText($"EmailSender.credentials.{credentialsFileName}");
            var model = JsonConvert.DeserializeObject<EmailConfigurationModel>(fileContent);

            return model;
        }
    }
}
