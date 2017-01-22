using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Core.Doctors;
using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DPTS.Data.Context;
using DPTS.Domain.Entities;

namespace DPTS.Web.Controllers
{
    public class AppointmentController : BaseController
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _scheduleService;
        private readonly DPTSDbContext _context;
        #endregion

        #region Contr
        public AppointmentController(IDoctorService doctorService,
            IAppointmentService scheduleService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _context = new DPTSDbContext();
        }

        #endregion

        #region Utilities
        private static AppointmentScheduleViewModel GenrateTimeSlots(string startTime, string endTime, double duration,IList<AppointmentSchedule> bookedSlots )
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
                    //if (start < DateTime.Parse("12:00 PM"))
                    //{
                        var morn = new MorningSlotModel
                        {
                            Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                        };
                        slots.Morning.Add(morn);
                   // }
                    //else if (start > DateTime.Parse("06:00 PM"))
                    //{
                    //    var eve = new EveningSlotModel
                    //    {
                    //        Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                    //    };
                    //    slots.Evening.Add(eve);
                    //}
                    //else
                    //{
                    //    var aft = new AfternoonSlotModel
                    //    {
                    //        Slot = start.ToShortTimeString() + " - " + dtNext.ToShortTimeString()
                    //    };
                    //    slots.Afternoon.Add(aft);
                    //}
                    start = dtNext;
                }
                return slots;
            }
            catch { return null; }
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult Booking(string doctorId, string selectedDate = null)
        {
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                var scheduleSlots = new AppointmentScheduleViewModel();

                string todayDay = DateTime.UtcNow.ToString("dddd");
                var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).FirstOrDefault(s => s.Day.Equals(todayDay));
                if (schedule == null)
                    return RedirectToAction("NoSchedule");

                var bookedSlots = _scheduleService.GetAppointmentScheduleByDoctorId(doctorId);

                scheduleSlots = GenrateTimeSlots("1:00 AM", "2:00 PM", 20);
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
                return RedirectToAction("VisitorContactDeatils", new { doctorId = model.doctorId });
            }
            return View();
        }

        public ActionResult NoSchedule()
        {
            return View();
        }

        public JsonResult VisitorContactDeatils()
        {
            if (!Request.IsAuthenticated)
                return Json(new
                {
                    success = 1
                });

            var userId = User.Identity.GetUserId();
            var visitor = _context.AspNetUsers.SingleOrDefault(u => u.Id == userId);
            if (visitor == null)
            {
                return Json(new
                {
                    success =1
                });
            }
            var fullName = visitor.FirstName + " " + visitor.LastName; 
            return Json(data: new
            {
                mobilenumber = visitor.PhoneNumber,
                useremail = visitor.Email,
                username = fullName
            });
        }
        public JsonResult PaymentMode()
        {
            return Json(new
            {
                success = 1
            });
        }
        //booking_date,"slottime" ,,"subject","username","mobilenumber","useremail","booking_note","payment"

        public JsonResult FinishBooking(FormCollection form)
        {
            if(!string.IsNullOrWhiteSpace(form["booking_date"]) &&
                !string.IsNullOrWhiteSpace("slottime") &&
                !string.IsNullOrWhiteSpace("subject") &&
                !string.IsNullOrWhiteSpace("username") &&
                !string.IsNullOrWhiteSpace("mobilenumber") &&
                !string.IsNullOrWhiteSpace("useremail") &&
                !string.IsNullOrWhiteSpace("booking_note") &&
                !string.IsNullOrWhiteSpace("payment") &&
                !string.IsNullOrWhiteSpace("doctorId") &&
                Request.IsAuthenticated)
            {
                string statusFlag = "Pending";
                var bookingStatus = _scheduleService.GetAppointmentStatusByName(statusFlag);
                var userId = User.Identity.GetUserId();

                var booking = new AppointmentSchedule
                {
                    DoctorId = form["doctorId"],
                    PatientId = userId,
                    Subject = form["subject"],
                    DiseasesDescription = form["booking_note"],
                    AppointmentTime = form["slottime"].Trim(),
                    StatusId = bookingStatus.Id,
                    DateCreated = DateTime.Parse(form["booking_date"])
                };
                _scheduleService.InsertAppointmentSchedule(booking);
                return Json(new
                {
                    success = 1
                });
            }
            return Json(new
            {
                success = 1
            });
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
        public JsonResult OpcSaveShipping()
        {
            return Json(new
            {
                success = 1
            });
        }
        #endregion

    }

    internal class UpdateSectionJsonModel
    {
        public object html { get; set; }
        public string name { get; set; }
    }
}