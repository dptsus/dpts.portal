using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private ApplicationDbContext context;

        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
            context = new ApplicationDbContext();
        }
        #endregion

        #region Utilities
        [NonAction]
        private List<SelectListItem> GetGender()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Select Gender", Value = "0" });
            foreach (var gender in Enum.GetValues(typeof(Gender)))
            {
                items.Add(new SelectListItem()
                {
                    Text = Enum.GetName(typeof(Gender), gender),
                    Value = Enum.GetName(typeof(Gender), gender)
                });
            }
            return items;
        }
        #endregion

        #region Methods
        public async Task<ActionResult> Info()
        {
            var data = await _doctorService.GetAllDoctorAsync(showhidden: true, enableTracking: true);
            return View();
        }

        public ActionResult ProfileSetting()
        {
            var model = new DoctorProfileSettingViewModel();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                model.Email = user.Email;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Id = user.Id;
                model.PhoneNumber = user.PhoneNumber;
            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ProfileSetting(DoctorProfileSettingViewModel model)
        {
            var data = await _doctorService.GetDoctorbyIdAsync(model.Id);
            if (data == null)
                return null;

            var doctor = new Doctor();
            doctor.Gender = model.Gender;
            doctor.DoctorId = model.Id;
            doctor.ShortProfile = model.ShortProfile;
            doctor.DateOfBirth = model.DateOfBirth;

            _doctorService.AddDoctorAsync(doctor);

           return RedirectToAction("Info");
        }
        #endregion

    }
}