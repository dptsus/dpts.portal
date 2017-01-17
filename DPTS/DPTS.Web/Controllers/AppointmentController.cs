using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Core.Doctors;
using DPTS.Web.Models;
using System;
using System.Linq;
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
        private static AppointmentScheduleViewModel GetEndTime(string startTime, string endTime, double duration)
        {
            var slots = new AppointmentScheduleViewModel();

            var start = DateTime.Parse(startTime);
            var end = DateTime.Parse(endTime);
            while (true)
            {
                var dtNext = start.AddMinutes(duration);
                if (start > end || dtNext > end)
                    break;
                if (start < DateTime.Parse("12:00 PM"))
                {
                    var morn = new MorningSlotModel
                    {
                        Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                    };
                    slots.Morning.Add(morn);
                }
                else
                {
                    var aft = new AfternoonSlotModel
                    {
                        Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                    };
                    slots.Afternoon.Add(aft);
                }
                start = dtNext;
            }

            //Label lbl = new Label();
            //lbl.Text = morning + afternon;
            //Form.Controls.Add(lbl);

            return null;
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult AppointmentSchedule(string doctorId,string selectedDate = null)
        {
            string todayDay = DateTime.UtcNow.ToString("dddd");
            var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).FirstOrDefault(s => s.Day.Equals(todayDay));
            if (schedule == null)
                return null;
            var scheduleSlots = GetEndTime(schedule.StartTime, schedule.EndTime, 10);
           // schedule.StartTime
            return View();
        }
        #endregion

    }
}