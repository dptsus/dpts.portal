namespace DPTS.Domain.Core.Notification
{
    /// <summary>
    /// Registration Notification Interface
    /// </summary>
    public interface IRegistrationNotificationService
    {
        /// <summary>
        /// send registration notification
        /// </summary>
        void SendRegistrationNotification(string userEmail, string optionaluserMobileNumber = "DoNotSendSMS");

        /// <summary>
        /// send OTP on Registration
        /// </summary>
        string SendRegistrationOTP(string userMobileNumber, string optionaluserMobileEmail = "DoNotSendEmail");
    }
}