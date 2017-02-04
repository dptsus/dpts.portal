using DPTS.Domain.Core.Notification;
using DPTS.EmailSmsNotifications.IServices;
using DPTS.EmailSmsNotifications.ServiceModels;

namespace DPTS.Domain.Notification
{
    class RegistrationNotificationService : IRegistrationNotificationService
    {
        #region Fields
        private readonly ISmsNotificationService _smsService;
        private readonly IEmailNotificationService _emailService;
        private readonly IDefaultNotificationSettingsService _defaultNotificationSettingsService;
        #endregion

        #region Constructor
        public RegistrationNotificationService(ISmsNotificationService smsService, 
            IEmailNotificationService emailService, 
            IDefaultNotificationSettingsService defaultNotificationSettingsService)
        {
            _smsService = smsService;
            _emailService = emailService;
            _defaultNotificationSettingsService = defaultNotificationSettingsService;
        }
        #endregion

        public void SendRegistrationNotification(string userEmail, 
            string optionaluserMobileNumber)
        {
            string content = 
                _defaultNotificationSettingsService.GetDefaultNotificationSettingsByName(
                    "RegistrationNotificationMessage").Message;

            if (optionaluserMobileNumber != "DoNotSendSMS")
            {
                SmsNotificationModel sms = new SmsNotificationModel();
                sms.numbers = optionaluserMobileNumber;
                sms.route = 4; //route 4 is for transactional sms
                sms.senderId = "DOCPTS";
                sms.message = content;
                _smsService.SendSms(sms);
            }

            EmailNotificationModel email = new EmailNotificationModel();
            email.from = "support@dpts.com";
            email.to = userEmail;
            email.subject = "Doc Direct Registration";
            email.content = content;
            _emailService.SendEmail(email);
        }

        public string SendRegistrationOTP(string userMobileNumber, string optionaluserMobileEmail)
        {
            SmsNotificationModel sms = new SmsNotificationModel();
            sms.numbers = userMobileNumber;
            sms.route = 4; //route 4 is for transactional sms
            sms.senderId = "DOCPTS";
            string otp = _smsService.GenerateOTP();
            sms.message = "DTPS Verification code: " + otp + "." + 
                "Pls do not share with anyone. It is valid for 10 minutes.";
            _smsService.SendSms(sms);
            return otp;
        }

    }
}
