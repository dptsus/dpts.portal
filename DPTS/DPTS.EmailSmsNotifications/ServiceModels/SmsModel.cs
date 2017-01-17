namespace DPTS.EmailSmsNotifications.ServiceModels
{
    public class SmsModel
    {
        public string numbers { get; set; }
        public string senderId { get; set; }
        public string message { get; set; }
        public int route { get; set; }
    }
}