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
        public IList<SelectListItem> GetAllSpecialities(string docterId)
        {
            var models = new List<SelectListItem>();
            var data = _specialityService.GetAllSpeciality(false);

            foreach(var speciality in data)
            {
                var specilityMap = new Doctor_Speciality_Mapping
                {
                    Speciality_Id = speciality.Id,
                    Doctor_Id = docterId
                };
                if (_specialityService.IsDoctorSpecialityExists(specilityMap))
                {
                    models.Add(new SelectListItem
                    {
                        Text = speciality.Title,
                        Value = speciality.Id.ToString(),
                        Selected=true
                    });
                }
                else
                {
                    models.Add(new SelectListItem
                    {
                        Text = speciality.Title,
                        Value = speciality.Id.ToString()
                    });
                }
            }

            return models;
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
                // model.Speciality = GetAllSpecialities();
                model.AvailableSpeciality = GetAllSpecialities(user.Id);


            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileSetting(DoctorProfileSettingViewModel model)
        {
           // var doctorSpec = GetAllSpecialities(model.Id).Select(c => c.Selected == true);

            var Specilities = string.Join(",", model.SelectedSpeciality);
            foreach (var item in Specilities.Split(',').ToList())
            {
                var specilityMap = new Doctor_Speciality_Mapping
                {
                    Speciality_Id = int.Parse(item),
                    Doctor_Id = model.Id,
                    DateCreated=DateTime.UtcNow,
                    DateUpdated=DateTime.UtcNow,
                };
                if (!_specialityService.IsDoctorSpecialityExists(specilityMap))
                {
                    _specialityService.AddSpecialityByDoctor(specilityMap);
                }
            }

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

        public ActionResult Favourites()
        {
            return View();
        }
        public ActionResult InvoicesPackages()
        {
            return View();
        }
        public ActionResult DoctorSchedules()
        {
            return View();
        }
        public ActionResult BookingListings()
        {
            return View();
        }
        public ActionResult BookingSchedules()
        {
            return View();
        }
        public ActionResult SecuritySettings()
        {
            return View();
        }
        public ActionResult PrivacySettings()
        {
            return View();
        }
        public ActionResult BookingSettings()
        {
            return View();
        }
        #endregion

    }
}