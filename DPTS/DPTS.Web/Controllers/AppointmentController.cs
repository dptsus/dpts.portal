using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Core.Doctors;
using DPTS.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class AppointmentController : BaseController
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
        private static AppointmentScheduleViewModel GenrateTimeSlots(string startTime, string endTime, double duration)
        {
            try
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
                    else if (start > DateTime.Parse("06:00 PM"))
                    {
                        var eve = new EveningSlotModel
                        {
                            Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                        };
                        slots.Evening.Add(eve);
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
                return slots;
            }
            catch { return null; }
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult AvailableSchedule(string doctorId,string selectedDate = null)
        {
            var scheduleSlots = new AppointmentScheduleViewModel();
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                string todayDay = DateTime.UtcNow.ToString("dddd");
                var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).FirstOrDefault(s => s.Day.Equals(todayDay));
                if (schedule == null)
                    return RedirectToAction("NoSchedule");

                scheduleSlots = GenrateTimeSlots(schedule.StartTime, schedule.EndTime, 20);
                scheduleSlots.doctorId = doctorId;
                return View(scheduleSlots);
            }
            return RedirectToAction("NoSchedule");

        }
        [HttpPost]
        public ActionResult AvailableSchedule(AppointmentScheduleViewModel model, string Command)
        {
            if (Command == "next" && !string.IsNullOrWhiteSpace(model.doctorId))
            {
                return RedirectToAction("VisitorContactDeatils",new { doctorId = model.doctorId });
            }
            return View();
        }

        public ActionResult VisitorContactDeatils(string doctorId)
        {
            var model = new VisitorContactDeatilsModel
            {
                doctorId =doctorId
            };
            return View(model);
        }
        public ActionResult NoSchedule()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VisitorContactDeatils(VisitorContactDeatilsModel model,string Command)
        {
            try
            {

                if (Command == "previous" && !string.IsNullOrWhiteSpace(model.doctorId))
                {
                    return RedirectToAction("AvailableSchedule", new { doctorId = model.doctorId });
                }
                else if (Command == "next")
                {
                    return RedirectToAction("PaymentMode");
                }
            }
            catch { }
            return View();
        }
        public ActionResult PaymentMode()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PaymentMode(string Command)
        {
            try
            {
                if (Command == "previous")
                {
                    return RedirectToAction("VisitorContactDeatils");
                }
                else if (Command == "next")
                {
                    return RedirectToAction("Finish");
                }
            }
            catch { }
            return View();
        }
        public ActionResult Finish()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Finish(string Command)
        {
            return View();
        }

        public ActionResult demoPicker()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult OpcSaveBilling(FormCollection form)
        {
            var model = new VisitorContactDeatilsModel
            {
                Name = "Tushar Khairnar"
            };
            if (model != null)
            {
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "billing",
                        html = this.RenderPartialViewToString("OpcBillingAddress", model)
                    },
                    wrong_billing_address = true,
                });
            }
            return View();
        }
        public ActionResult OpcSaveShipping(FormCollection form)
        {
            return View();
        }
        #endregion

    }

    internal class UpdateSectionJsonModel
    {
        public object html { get; set; }
        public string name { get; set; }
    }
}