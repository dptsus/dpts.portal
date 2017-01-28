namespace DPTS.Domain.Core.Notifications
{
    /// <summary>
    /// Appointment Notification Interface
    /// </summary>
    public interface IAppointmentNotificationService
    {
        ///<summary>
        ///send appointment schedule notification
        ///</summary>
        void SendAppointmentSchedule();
    }
}
