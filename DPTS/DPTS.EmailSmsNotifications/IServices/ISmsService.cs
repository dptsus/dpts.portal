using DPTS.EmailSmsNotifications.ServiceModels;

namespace DPTS.EmailSmsNotifications.IServices
{
    public interface ISmsService
    {
        string SendSms(SmsModel sms);
    }
}