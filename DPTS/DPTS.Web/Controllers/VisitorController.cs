using System;
using System.Web.Mvc;
using DPTS.Data.Context;
using DPTS.Domain.Core.Appointment;
using DPTS.Web.Models;
using DPTS.Domain.Core.Doctors;
using Microsoft.AspNet.Identity;

namespace DPTS.Web.Controllers
{
    public class VisitorController : BaseController
    {
        #region Fields

        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _scheduleService;
        private readonly DPTSDbContext _context;

        #endregion

        #region Contr

        public VisitorController(IDoctorService doctorService,
            IAppointmentService scheduleService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _context = new DPTSDbContext();
        }

        #endregion

        #region Methods

        public ActionResult AppointmentList()
        {
            var model = new VisitorViewModel();

            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var visitorSchedule = _scheduleService.GetAppointmentScheduleByPatientId(userId);
                model.AppointmentSchedule = visitorSchedule;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CancelAppoinmant(int appoinmentId)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    var appoinment = _scheduleService.GetAppointmentScheduleById(appoinmentId);
                    if (appoinment == null)
                        return HttpNotFound();

                    var status = _scheduleService.GetAppointmentStatusByName("Cancelled");
                    if (status == null)
                        return HttpNotFound();

                    appoinment.StatusId = status.Id;
                    _scheduleService.UpdateAppointmentSchedule(appoinment);
                    return Json(new
                    {
                        redirect = Url.Action("AppointmentList", "Visitor"),
                    });
                }
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        #endregion
    }
}