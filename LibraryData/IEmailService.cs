using System.Threading.Tasks;

namespace LibraryData
{
    public interface IEmailService
    {
         Task SendEmailAsync(string toName,
                             string toEmailAddress,
                             string subject,
                             string message);
    }
}
