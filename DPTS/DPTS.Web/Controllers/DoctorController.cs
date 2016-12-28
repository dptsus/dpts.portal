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
        private readonly ISpecialityService _specialityService;
        private ApplicationDbContext context;

        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService, ISpecialityService specialityService)
        {
            _doctorService = doctorService;
            context = new ApplicationDbContext();
            _specialityService = specialityService;
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
        public IList<Speciality> GetAllSpecialities()
        {
            List<Speciality> lstSpecilaity = new List<Speciality>();
            var data = _specialityService.GetAllSpeciality(false);
            if (data == null)
                return null;

            foreach(var speciality in data)
            {
                Speciality spec = new Speciality();
                spec.Title = speciality.Title;
                spec.Id = speciality.Id;
                spec.IsActive = speciality.IsActive;
                spec.DisplayOrder = speciality.DisplayOrder;
                lstSpecilaity.Add(spec);
            }

            return lstSpecilaity;
        }
        #endregion

        #region Methods
        public async Task<ActionResult> Info()
        {
           // var data = await _doctorService.ge(showhidden: true, enableTracking: true);
            return View();
        }

        public ActionResult ProfileSetting()
        {
            var model = new DoctorProfileSettingViewModel();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                var doctor = _doctorService.GetDoctorbyId(user.Id);
                if(doctor != null)
                {
                    model.DateCreated = doctor.DateCreated;
                    model.DateOfBirth = doctor.DateOfBirth;
                    model.Gender = doctor.Gender;
                    model.ShortProfile = doctor.ShortProfile;
                }
                model.Email = user.Email;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Id = user.Id;
                model.PhoneNumber = user.PhoneNumber;
                model.Speciality = GetAllSpecialities();

            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileSetting(DoctorProfileSettingViewModel model)
        {
            var doctor =  _doctorService.GetDoctorbyId(model.Id);
            if (doctor == null)
                return null;

            doctor.Gender = model.Gender;
            doctor.ShortProfile = model.ShortProfile;
            doctor.DateOfBirth = model.DateOfBirth;
            doctor.DateUpdated = DateTime.UtcNow;
            _doctorService.UpdateDoctor(doctor);

            return RedirectToAction("Info");
        }

        #endregion

    }
}