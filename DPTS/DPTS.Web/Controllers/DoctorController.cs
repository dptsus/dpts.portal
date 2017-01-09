using DPTS.Domain;
using DPTS.Domain.Core;
using DPTS.Web.Models;
using SQS_Shopee.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Entities;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly ISpecialityService _specialityService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private ApplicationDbContext context;
        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService, ISpecialityService specialityService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressService addressService)
        {
            _doctorService = doctorService;
            context = new ApplicationDbContext();
            _specialityService = specialityService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _addressService = addressService;
        }
        #endregion

        #region Utilities
        [NonAction]
        public ApplicationUser GetUserById(string userId)
        {
            return context.Users.SingleOrDefault(u => u.Id == userId);
        }
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
                var specilityMap = new SpecialityMapping
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(int countryId, bool addSelectStateItem)
        {
            //this action method gets called via an ajax request
            if (countryId == 0)
                throw new ArgumentNullException("countryId");

            var country = _countryService.GetCountryById(countryId);
            var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0).ToList();
            var result = (from s in states
                          select new { id = s.Id, name = s.Name })
                          .ToList();


            if (country == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem)
                {
                    result.Insert(0, new { id = 0, name = "select state" });
                }
                else
                {
                    result.Insert(0, new { id = 0, name = "None" });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new { id = 0, name = "None" });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = "select state" });
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IList<SelectListItem> GetCountryList()
        {
            var countries = _countryService.GetAllCountries(true);
            List<SelectListItem> typelst = new List<SelectListItem>();
            typelst.Add(
                     new SelectListItem
                     {
                         Text = "Select",
                         Value = "0"
                     });
            foreach (var _type in countries.ToList())
            {
                typelst.Add(
                     new SelectListItem
                     {
                         Text = _type.Name,
                         Value = _type.Id.ToString()
                     });
            }
            return typelst;
        }
        #endregion

        #region Methods
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ProfileSetting()
        {
            var model = new DoctorProfileSettingViewModel();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
               // var user = context.Users.a(u => u.Id).Where(u => u.Id == id).FirstOrDefault();

                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                var doctor = _doctorService.GetDoctorbyId(user.Id);
                if(doctor != null)
                {
                    model.DateCreated = doctor.DateCreated;
                    var dateOfBirth = string.IsNullOrWhiteSpace(doctor.DateOfBirth) ? (DateTime?)null : DateTime.Parse(doctor.DateOfBirth);
                    if (dateOfBirth.HasValue)
                    {
                        model.DateOfBirthDay = dateOfBirth.Value.Day;
                        model.DateOfBirthMonth = dateOfBirth.Value.Month;
                        model.DateOfBirthYear = dateOfBirth.Value.Year;
                    }
                    model.Gender = doctor.Gender;
                    model.ShortProfile = doctor.ShortProfile;
                    model.Qualifications = doctor.Qualifications;
                //    model.NoOfYearExperience = doctor.YearsOfExperience;
                    model.RegistrationNumber = doctor.RegistrationNumber;

                }
                model.Email = user.Email;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Id = user.Id;
                model.PhoneNumber = user.PhoneNumber;
                model.AvailableSpeciality = GetAllSpecialities(user.Id);
            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileSetting(DoctorProfileSettingViewModel model)
        {
            var dob = model.DateOfBirth;
            // var doctorSpec = GetAllSpecialities(model.Id).Select(c => c.Selected == true);
            if (model.SelectedSpeciality.Count > 0)
            {
                var Specilities = string.Join(",", model.SelectedSpeciality);
                foreach (var item in Specilities.Split(',').ToList())
                {
                    var specilityMap = new SpecialityMapping
                    {
                        Speciality_Id = int.Parse(item),
                        Doctor_Id = model.Id,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                    };
                    if (!_specialityService.IsDoctorSpecialityExists(specilityMap))
                    {
                        _specialityService.AddSpecialityByDoctor(specilityMap);
                    }
                }
            }

            var doctor =  _doctorService.GetDoctorbyId(model.Id);
            if (doctor == null)
                return null;

            doctor.Gender = model.Gender;
            doctor.ShortProfile = model.ShortProfile;
            DateTime? dateOfBirth = model.ParseDateOfBirth();
            doctor.DateOfBirth = dateOfBirth.ToString();
            doctor.DateUpdated = DateTime.UtcNow;
            doctor.Qualifications = model.Qualifications;
            doctor.RegistrationNumber = model.RegistrationNumber;
            doctor.YearsOfExperience = model.NoOfYearExperience;
            _doctorService.UpdateDoctor(doctor);

            return RedirectToAction("Info");
        }

        public ActionResult AddressAdd()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var model = new AddressViewModel();
            model.AvailableCountry = GetCountryList();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddressAdd(AddressViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            if(ModelState.IsValid)
            {
                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);

                var address = new Address
                {
                    StateProvinceId=model.StateProvinceId,
                    CountryId =model.CountryId,
                    Address1=model.Address1,
                    Address2=model.Address2,
                    Hospital=model.Hospital,
                    FaxNumber=model.FaxNumber,
                    PhoneNumber=model.LandlineNumber,
                    Website=model.Website,
                    ZipPostalCode=model.ZipPostalCode,
                    City=model.City
                };
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                 _addressService.AddAddress(address);
                var addrMap = new AddressMapping
                {
                    AddressId = address.Id,
                    UserId = user.Id
                };
                _addressService.AddAddressMapping(addrMap);

                return RedirectToAction("Addresses");
            }
            model.AvailableCountry = GetCountryList();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddressDelete(int addressId)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
            var address = _addressService.GetAddressbyId(addressId);
            if (address != null)
            {
                _addressService.DeleteAddress(address);
                var addrMapping = _addressService.GetAddressMappingbuUserIdAddrId(UserId: user.Id, AddressId: addressId);
                if (addrMapping == null)
                    return Content("No Customer found with the specified id");

                _addressService.DeleteAddressMapping(addrMapping);
            }

            return Json(new
            {
                redirect = Url.Action("Addresses","Doctor"),
            });
        }

        public ActionResult Addresses()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();


            var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
            var model = new List<AddressViewModel>();

            model.AddRange(_addressService.GetAllAddressByUser(user.Id).Select(c => new AddressViewModel
            {
                Id = c.Id,
                Address1 = c.Address1,
                Address2 = c.Address2,
                City = c.City,
                CountryName = (c.CountryId.GetValueOrDefault() > 0) ? _countryService.GetCountryById(c.CountryId.GetValueOrDefault()).Name : " ",
                StateName = (c.StateProvinceId.GetValueOrDefault() > 0) ? _stateProvinceService.GetStateProvinceById(c.StateProvinceId.GetValueOrDefault()) .Name : " ",
                FaxNumber=c.FaxNumber,
                Hospital=c.Hospital,
                LandlineNumber=c.PhoneNumber,
                Website=c.Website,
                ZipPostalCode=c.ZipPostalCode,

            }).ToList());

            return View(model);

        }

        public ActionResult AddressEdit(int Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var address = _addressService.GetAddressbyId(Id);
            if (address == null)
                return null;

            var model = new AddressViewModel
            {
                Id=address.Id,
                Address1=address.Address1,
                Address2=address.Address2,
                City=address.City,
                CountryId=address.CountryId.GetValueOrDefault(),
                FaxNumber=address.FaxNumber,
                Hospital=address.Hospital,
                LandlineNumber=address.PhoneNumber,
                StateProvinceId=address.StateProvinceId.GetValueOrDefault(),
                Website=address.Website,
                ZipPostalCode=address.ZipPostalCode,
                AvailableCountry=GetCountryList(),
              //  AvailableStateProvince= GetStatesByCountryId(address.CountryId)

            };

                //states

                var states = _stateProvinceService
                    .GetStateProvincesByCountryId(model.CountryId)
                    .ToList();
                if (states.Any())
                {
                    model.AvailableStateProvince.Add(new SelectListItem { Text = "Select state", Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStateProvince.Add(new SelectListItem
                        {
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = (s.Id == model.StateProvinceId)
                        });
                    }
                }
                else
                {
                    bool anyCountrySelected = model.AvailableCountry.Any(x => x.Selected);
                    model.AvailableStateProvince.Add(new SelectListItem
                    {
                        Text = (anyCountrySelected ? "None" : "Select State"),
                        Value = "0"
                    });
                }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(AddressViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var address = _addressService.GetAddressbyId(model.Id);
            address.Id = model.Id;
            address.Address1 = model.Address1;
            address.Address2 = model.Address2;
            address.City = model.City;
            address.CountryId = model.CountryId;
            address.FaxNumber = model.FaxNumber;
            address.Hospital = model.Hospital;
            address.PhoneNumber = model.LandlineNumber;
            address.StateProvinceId = model.StateProvinceId;
            address.Website = model.Website;
            address.ZipPostalCode = model.ZipPostalCode;

            _addressService.UpdateAddress(address);
            return RedirectToAction("Addresses");
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
        public ContentResult UploadFiles()
        {
            try
            {
                var r = new List<UploadFilesResult>();
                string prodId = string.Empty;
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    prodId = Request.QueryString["pid"].ToString() + Path.GetExtension(hpf.FileName);
                    var folderPath = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string savedFileName = Path.Combine(folderPath, prodId);
                    hpf.SaveAs(savedFileName);

                    r.Add(new UploadFilesResult()
                    {
                        Name = hpf.FileName,
                        Length = hpf.ContentLength,
                        Type = hpf.ContentType
                    });
                }

                return Content("{\"name\":\"" + prodId + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ActionResult DoctorDetails(string doctorId)
        {
            var model = new DoctorViewModel();
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                var doctor = _doctorService.GetDoctorbyId(doctorId);
                if (doctor == null)
                    return null;

                var user = GetUserById(doctor.DoctorId);
                if (user == null)
                    return null;

                model.Id = user.Id;
                model.Email = user.Email;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.MobileNumber = user.PhoneNumber;
                model.doctor = doctor;
                model.Addresses = _addressService.GetAllAddressByUser(doctor.DoctorId);
                model.Specialitys = _specialityService.GetDoctorSpecilities(doctor.DoctorId);

                return View(model);
            }
            return View();
        }

        #endregion

    }
}