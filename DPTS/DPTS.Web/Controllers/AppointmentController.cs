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
using System.Text;
using DPTS.EmailSmsNotifications.ServiceModels;
using DPTS.EmailSmsNotifications.IServices;
using DPTS.Domain.Core.Address;

namespace DPTS.Web.Controllers
{
    public class AppointmentController : BaseController
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _scheduleService;
        private readonly DPTSDbContext _context;
        private ISmsNotificationService _smsService;
        private IEmailNotificationService _emailService;
        private readonly IAddressService _addressService;
        #endregion

        #region Contr
        public AppointmentController(IDoctorService doctorService,
            IAppointmentService scheduleService, ISmsNotificationService smsService,
            IEmailNotificationService emailService, IAddressService addressService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _context = new DPTSDbContext();
            _smsService = smsService;
            _emailService = emailService;
            _addressService = addressService;
        }

        #endregion

        #region Utilities
        private static AppointmentScheduleViewModel GenrateTimeSlots(string startTime,
            string endTime, double duration,
            IList<AppointmentSchedule> bookedSlots)
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

                    var slot = start.ToString("hh:mm tt") + " - " + dtNext.ToString("hh:mm tt");
                    var bookingstatus = bookedSlots.Where(s => s.AppointmentTime == slot).FirstOrDefault();

                    var splitSlot = new ScheduleSlotModel
                    {
                        Slot = slot,
                        IsBooked = (bookingstatus == null) ? false : true
                    };
                    slots.ScheduleSlotModel.Add(splitSlot);
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
            try
            {
                if (!string.IsNullOrWhiteSpace(doctorId))
                {
                    string todayDay = DateTime.UtcNow.ToString("dddd");
                    var schedule =
                        _scheduleService.GetScheduleByDoctorId(doctorId).FirstOrDefault(s => s.Day.Equals(todayDay));
                    if (schedule == null)
                        return RedirectToAction("NoSchedule");

                    var bookedSlots =
                        _scheduleService.GetAppointmentScheduleByDoctorId(doctorId)
                            .Where(s => s.AppointmentDate.Equals(DateTime.UtcNow.ToString("yyyy-MM-dd")))
                            .ToList();
                    bookedSlots =
                        bookedSlots.Where(
                            s => s.AppointmentStatus.Name == "Pending" || s.AppointmentStatus.Name == "Booked")
                            .ToList();

                    var scheduleSlots = GenrateTimeSlots(schedule.StartTime, schedule.EndTime, 20, bookedSlots);
                    scheduleSlots.doctorId = doctorId;
                    return View(scheduleSlots);
                }
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("NoSchedule");

        }

        public JsonResult BookingScheduleByDate(string slot_date, string doctorId)
        {
            var response = new StringBuilder();
            string resultNoResultFound = string.Empty;

            if (!string.IsNullOrWhiteSpace(doctorId) && !string.IsNullOrWhiteSpace(slot_date))
            {
                resultNoResultFound += "<div class=\"bk-thanks-message\">";
                resultNoResultFound += "<div class=\"tg-message\">";
                resultNoResultFound += "<h2>Schedule Not Found !!</h2>";
                resultNoResultFound += "<div class=\"tg-description\">";
                resultNoResultFound += "<p>Appointment not available...</p>";
                resultNoResultFound += "</div></div></div>";

                string todayDay = DateTime.Parse(slot_date).ToString("dddd");
                //DateTime.UtcNow.ToString("dddd");
                var schedule = _scheduleService.GetScheduleByDoctorId(doctorId).FirstOrDefault(s => s.Day.Equals(todayDay));
                if (schedule == null)
                {
                    return Json(new
                    {
                        result = resultNoResultFound
                    });
                }
                var bookedSlots = _scheduleService.GetAppointmentScheduleByDoctorId(doctorId).Where(s => s.AppointmentDate.Equals(slot_date)).ToList();

                bookedSlots =
                        bookedSlots.Where(
                            s => s.AppointmentStatus.Name == "Pending" || s.AppointmentStatus.Name == "Booked")
                            .ToList();


                var scheduleSlots = GenrateTimeSlots(schedule.StartTime, schedule.EndTime, 20, bookedSlots);

                if (scheduleSlots == null)
                {

                    return Json(new
                    {
                        result = resultNoResultFound
                    });
                }

                foreach (var item in scheduleSlots.ScheduleSlotModel)
                {
                    string repo = string.Empty;
                    if (item.IsBooked)
                    {
                        repo = "<div class=\"tg-doctimeslot tg-booked\">";
                        repo += "<div class=\"tg-box\">";
                        repo += "<div class=\"tg-radio\">";
                        repo += "<input id = \"" + item.Slot + "\" value=\"" + item.Slot + "\" type=\"radio\" name=\"slottime\" disabled >";
                        repo += "<label for=\"" + item.Slot + "\">" + item.Slot + "</label>";
                        repo += "</div> </div> </div>";
                    }
                    else
                    {
                        repo = "<div class=\"tg-doctimeslot tg-available\">";
                        repo += "<div class=\"tg-box\">";
                        repo += "<div class=\"tg-radio\">";
                        repo += "<input id = \"" + item.Slot + "\" value=\"" + item.Slot + "\" type=\"radio\" name=\"slottime\">";
                        repo += "<label for=\"" + item.Slot + "\">" + item.Slot + "</label>";
                        repo += "</div> </div> </div>";
                    }
                    response.AppendLine(repo);
                }
                return Json(new
                {
                    response = response.ToString()
                });
            }
            return Json(new
            {
                success = 1
            });
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
                    success = 1
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
            if (!string.IsNullOrWhiteSpace(form["booking_date"]) &&
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
                    AppointmentDate = form["booking_date"]
                };

                _scheduleService.InsertAppointmentSchedule(booking);

                //Below is the temporary code, i will update service class, interfaces

                Doctor doctorDetails = _doctorService.GetDoctorbyId(booking.DoctorId);
                string doctorName = "Dr. " + doctorDetails.AspNetUser.FirstName + doctorDetails.AspNetUser.LastName;
                string appointmemtSchedule = booking.AppointmentDate + " " + booking.AppointmentTime;
                string appomitmentAddress = _addressService.GetAllAddressByUser(booking.DoctorId).FirstOrDefault().Address1 + " " +
                    _addressService.GetAllAddressByUser(booking.DoctorId).FirstOrDefault().Address2 + " " +
                    _addressService.GetAllAddressByUser(booking.DoctorId).FirstOrDefault().City + " " +
                    _addressService.GetAllAddressByUser(booking.DoctorId).FirstOrDefault().ZipPostalCode;
                string contactNumber = doctorDetails.AspNetUser.PhoneNumber;

                SmsNotificationModel sms = new SmsNotificationModel();
                sms.numbers = form["mobilenumber"].ToString();
                sms.route = 4; //route 4 is for transactional sms
                sms.senderId = "DOCPTS";
                sms.message = "Your appointment with " + doctorName + " is scheduled for " +
                    appointmemtSchedule + ", " + appomitmentAddress + ", " + contactNumber;
                _smsService.SendSms(sms);

                EmailNotificationModel email = new EmailNotificationModel();
                email.from = doctorDetails.AspNetUser.Email;
                email.to = form["useremail"].ToString();
                email.subject = "Doc Direct Appointment Schedule.";
                email.content = "Your appointment with " + doctorName + " is scheduled for " +
                    appointmemtSchedule + ", " + appomitmentAddress + ", " + contactNumber;
                _emailService.SendEmail(email);

                return Json(new
                {
                    result = "success"
                });
            }
            return Json(new
            {
                result = "fail"
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
                action_type = "cancelled"
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