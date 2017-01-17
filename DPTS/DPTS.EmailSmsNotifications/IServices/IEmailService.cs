using DPTS.EmailSmsNotifications.ServiceModels;
using System.Threading.Tasks;

namespace DPTS.EmailSmsNotifications.IServices
{
    public interface IEmailService
    {
        Task SendEmail(EmailModel model);
    }
}