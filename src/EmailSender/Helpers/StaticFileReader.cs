using System.IO;
using System.Reflection;
using System.Text;

namespace EmailSender
{
    public class StaticFileReader
    {
        public static string GetFileText(string fileResource)
        {
            var assembly = typeof(HtmlContentProvider).GetTypeInfo().Assembly;
            var resourceStream = assembly.GetManifestResourceStream(fileResource);
            var fileContent = string.Empty;
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                fileContent = reader.ReadToEnd();
            }

            return fileContent;
        }
    }
}
