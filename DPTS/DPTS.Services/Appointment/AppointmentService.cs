using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain.Core;
using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Entities;

namespace DPTS.Services.Appointment
{
    public class AppointmentService : IAppointmentService
    {
        #region Fields
        private readonly IRepository<AppointmentSchedule> _scheduleRepository;
        #endregion

        #region Ctor
        public AppointmentService(IRepository<AppointmentSchedule> scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
        #endregion

        #region Methods
        public void DeleteAppointmentSchedule(AppointmentSchedule schedule)
        {
            if(schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            _scheduleRepository.Delete(schedule);
        }

        public IList<AppointmentSchedule> GetAllAppointmentSchedule()
        {
            var query = _scheduleRepository.Table;
            return query.ToList();
        }

        public AppointmentSchedule GetAppointmentScheduleById(int scheduleId)
        {
            if (scheduleId == 0)
                return null;

            return _scheduleRepository.GetById(scheduleId);
        }

        public IList<AppointmentSchedule> GetAppointmentScheduleByDoctorId(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
                return null;

            var query = from c in _scheduleRepository.Table
                where c.DoctorId.Equals(doctorId)
                select c;

            return query.ToList();
        }

        public IList<AppointmentSchedule> GetAppointmentScheduleByIds(int[] scheduleIds)
        {
            if (scheduleIds == null || scheduleIds.Length == 0)
                return new List<AppointmentSchedule>();

            var query = from c in _scheduleRepository.Table
                where scheduleIds.Contains(c.Id)
                select c;
            var schedules = query.ToList();
            return scheduleIds.Select(id => schedules.Find(x => x.Id == id)).Where(schedule => schedule != null).ToList();
        }

        public void InsertAppointmentSchedule(AppointmentSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            _scheduleRepository.Insert(schedule);

        }

        public void UpdateAppointmentSchedule(AppointmentSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            _scheduleRepository.Update(schedule);
        }
        #endregion
    }
}
