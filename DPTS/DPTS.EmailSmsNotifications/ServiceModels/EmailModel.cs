namespace DPTS.EmailSmsNotifications.ServiceModels
{
    public class EmailModel
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
    }
}