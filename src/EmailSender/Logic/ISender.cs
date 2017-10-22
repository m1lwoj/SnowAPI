using System.Threading.Tasks;

namespace EmailSender
{
    public interface ISender
    {
        Task SendCodeEmail(ContentType type, string receiverEmail, string code);
    }
}
