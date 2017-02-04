namespace DPTS.Domain.Core.Notification
{
    /// <summary>
    /// Appointment Notification Interface
    /// </summary>
    public interface IAppointmentNotificationService
    {
        ///<summary>
        ///send appointment schedule notification
        ///</summary>
        void SendAppointmentSchedule(string bookingDoctorId, string bookingAppointmentDate,
            string bookingAppointmentTime, string userEmail, string userMobileNumber);
        //void SendAppointmentSchedule(AppointmentScheduleDetails scheduleDetails);

    }
}
