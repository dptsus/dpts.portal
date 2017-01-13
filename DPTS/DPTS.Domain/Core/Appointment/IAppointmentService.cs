using System.Collections.Generic;
using DPTS.Domain.Entities;

namespace DPTS.Domain.Core.Appointment
{
    /// <summary>
    /// Appointment service interface
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>
        /// Deletes a Appointment
        /// </summary>
        /// <param name="schedule">Appointment</param>
        void DeleteAppointmentSchedule(AppointmentSchedule schedule);

        /// <summary>
        /// Gets all Appointment Schedule
        /// </summary>
        /// <returns>AppointmentSchedule</returns>
        IList<AppointmentSchedule> GetAllAppointmentSchedule();

        /// <summary>
        /// Gets a AppointmentSchedule
        /// </summary>
        /// <param name="scheduleId">AppointmentSchedule identifier</param>
        /// <returns>AppointmentSchedule</returns>
        AppointmentSchedule GetAppointmentScheduleById(int scheduleId);

        /// <summary>
        /// Get Appointment Schedules
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        IList<AppointmentSchedule> GetAppointmentScheduleByDoctorId(string doctorId);

        /// <summary>
        /// Get Appointment Schedule by identifiers
        /// </summary>
        /// <param name="scheduleIds">Country identifiers</param>
        /// <returns>Countries</returns>
        IList<AppointmentSchedule> GetAppointmentScheduleByIds(int[] scheduleIds);

        /// <summary>
        /// Inserts a Appointment Schedule
        /// </summary>
        /// <param name="schedule">Country</param>
        void InsertAppointmentSchedule(AppointmentSchedule schedule);

        /// <summary>
        /// Updates the Appointment Schedule
        /// </summary>
        /// <param name="schedule">Country</param>
        void UpdateAppointmentSchedule(AppointmentSchedule schedule);
    }
}