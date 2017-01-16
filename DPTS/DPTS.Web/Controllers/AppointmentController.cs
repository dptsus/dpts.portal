using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Core.Doctors;
using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class AppointmentController : Controller
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _scheduleService;
        private ApplicationDbContext context;
        #endregion

        #region Contr
        public AppointmentController(IDoctorService doctorService,
            IAppointmentService scheduleService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            context = new ApplicationDbContext();
        }

        #endregion

        #region Methods
        public ActionResult AppointmentSchedule(string doctorId,string selectedDate = null)
        {
            string todayDay = DateTime.UtcNow.ToString("dddd");
            var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).Where(s => s.Day.Equals(todayDay)).FirstOrDefault();
            if (schedule == null)
                return null;

           // schedule.StartTime
            return View();
        }
        #endregion

    }
}