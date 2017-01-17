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

        #region Utilities
        private AppointmentScheduleViewModel GetEndTime(string startTime, string endTime, double duration)
        {
            var slots = new AppointmentScheduleViewModel();

            DateTime start = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string morning = "";
            string afternon = "";
            while (true)
            {
                DateTime dtNext = start.AddMinutes(duration);
                if (start > end || dtNext > end)
                    break;
                if (start < DateTime.Parse("12:00 PM"))
                {
                    var morn =new MorningSlotModel();
                    morn.Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString();
                    slots.Morning.Add(morn);
                }
                else
                {
                    var aft = new AfternoonSlotModel();
                    aft.Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString();
                    slots.Afternoon.Add(aft);
                }
                start = dtNext;
            }
            if (morning.Length > 0)
                morning = "<B>Morning</B><BR>" + morning;
            if (afternon.Length > 0)
                afternon = "<B>Afternon</B><BR>" + afternon;

            //Label lbl = new Label();
            //lbl.Text = morning + afternon;
            //Form.Controls.Add(lbl);

            //DateTime startDateTime = DateTime.Parse(startTime);
            //DateTime endDateTime = startDateTime.AddHours(duration);

            return null;
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult AppointmentSchedule(string doctorId,string selectedDate = null)
        {
            string todayDay = DateTime.UtcNow.ToString("dddd");
            var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).Where(s => s.Day.Equals(todayDay)).FirstOrDefault();
            if (schedule == null)
                return null;
            var tt = GetEndTime(schedule.StartTime, schedule.EndTime, 10);
           // schedule.StartTime
            return View();
        }
        #endregion

    }
}