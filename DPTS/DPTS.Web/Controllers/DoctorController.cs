using DPTS.Web.Models;
using SQS_Shopee.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Country;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Speciality;
using DPTS.Domain.Core.StateProvince;
using DPTS.Domain.Entities;
using Microsoft.AspNet.Identity;
using static System.DateTime;
using DPTS.Domain.Core.Appointment;

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
        private readonly IAppointmentService _scheduleService;
        private ApplicationDbContext context;
        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService, ISpecialityService specialityService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressService addressService, IAppointmentService scheduleService)
        {
            _doctorService = doctorService;
            context = new ApplicationDbContext();
            _specialityService = specialityService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _addressService = addressService;
            _scheduleService = scheduleService;
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
                items.Add(new SelectListItem
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
            var states = _stateProvinceService.GetStateProvincesByCountryId(country?.Id ?? 0).ToList();
            var result = (from s in states
                          select new { id = s.Id, name = s.Name })
                          .ToList();


            if (country == null)
            {
                //country is not selected ("choose country" item)
                result.Insert(0, addSelectStateItem ? new {id = 0, name = "select state"} : new {id = 0, name = "None"});
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
            var typelst = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                }
            };
            typelst.AddRange(countries.ToList().Select(type => new SelectListItem
            {
                Text = type.Name, Value = type.Id.ToString()
            }));
            return typelst;
        }

        private enum DayOfWeek
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
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
            if (Request.IsAuthenticated)
            {
               // var user = context.Users.a(u => u.id).Where(u => u.id == id).FirstOrDefault();

                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                if (user != null)
                {
                    var doctor = _doctorService.GetDoctorbyId(user.Id);
                    if(doctor != null)
                    {
                        model.DateCreated = doctor.DateCreated;
                        var dateOfBirth = string.IsNullOrWhiteSpace(doctor.DateOfBirth) ? (DateTime?)null : Parse(doctor.DateOfBirth);
                        if (dateOfBirth.HasValue)
                        {
                            model.DateOfBirthDay = dateOfBirth.Value.Day;
                            model.DateOfBirthMonth = dateOfBirth.Value.Month;
                            model.DateOfBirthYear = dateOfBirth.Value.Year;
                        }
                        model.Gender = doctor.Gender;
                        model.ShortProfile = doctor.ShortProfile;
                        model.Qualifications = doctor.Qualifications;
                        model.NoOfYearExperience = doctor.YearsOfExperience.GetValueOrDefault();
                        model.RegistrationNumber = doctor.RegistrationNumber;
                    }
                }
                if (user != null)
                {
                    model.Email = user.Email;
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Id = user.Id;
                    model.PhoneNumber = user.PhoneNumber;
                    model.AvailableSpeciality = GetAllSpecialities(user.Id);
                }
            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileSetting(DoctorProfileSettingViewModel model)
        {
            // var doctorSpec = GetAllSpecialities(model.id).Select(c => c.Selected == true);
            if (model.SelectedSpeciality.Count > 0)
            {
                var specilities = string.Join(",", model.SelectedSpeciality);
                foreach (var specilityMap in specilities.Split(',').ToList().Select(item => new SpecialityMapping
                {
                    Speciality_Id = int.Parse(item),
                    Doctor_Id = model.Id,
                    DateCreated = UtcNow,
                    DateUpdated = UtcNow
                }).Where(specilityMap => !_specialityService.IsDoctorSpecialityExists(specilityMap)))
                {
                    _specialityService.AddSpecialityByDoctor(specilityMap);
                }
            }

            var doctor =  _doctorService.GetDoctorbyId(model.Id);
            if (doctor == null)
                return null;

            doctor.Gender = model.Gender;
            doctor.ShortProfile = model.ShortProfile;
            var dateOfBirth = model.ParseDateOfBirth();
            doctor.DateOfBirth = dateOfBirth.ToString();
            doctor.DateUpdated = UtcNow;
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
                if (user != null)
                {
                    var addrMap = new AddressMapping
                    {
                        AddressId = address.Id,
                        UserId = user.Id
                    };
                    _addressService.AddAddressMapping(addrMap);
                }

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
            var userId = User.Identity.GetUserId();
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new
                {
                    redirect = Url.Action("Addresses", "Doctor"),
                });

            var addrMapping = _addressService.GetAddressMappingbuUserIdAddrId(UserId: user.Id, AddressId: addressId);

            if (addrMapping == null)
                return Content("No Customer found with the specified id");

            _addressService.DeleteAddressMapping(addrMapping);
            var address = _addressService.GetAddressbyId(addressId);
            if (address == null)
                return Json(new
                {
                    redirect = Url.Action("Addresses", "Doctor"),
                });
            _addressService.DeleteAddress(address);


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

            if (user != null)
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

        public ActionResult AddressEdit(int id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return new HttpUnauthorizedResult();

            var address = _addressService.GetAddressbyId(id);
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
            try
            {
                var lst = new List<SheduleViewModel>();
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var obj = new SheduleViewModel();
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId());
                    switch (day)
                    {
                        case DayOfWeek.Sunday:
                            var scheduleSunday = schedule.Where(s => s.Day.Equals("Sunday")).FirstOrDefault();
                            if (scheduleSunday == null || !scheduleSunday.Day.Equals("Sunday"))
                                obj.Day = "Sunday";
                            else
                            {
                                obj.DoctorId = scheduleSunday.DoctorId;
                                obj.Day = scheduleSunday.Day;
                                obj.EndTime = scheduleSunday.EndTime.ToString();
                                obj.StartTime = scheduleSunday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Monday:
                            var scheduleMonday = schedule.Where(s => s.Day.Equals("Monday")).FirstOrDefault();
                            if (scheduleMonday == null || !scheduleMonday.Day.Equals("Monday"))
                                obj.Day = "Monday";
                            else
                            {
                                obj.DoctorId = scheduleMonday.DoctorId;
                                obj.Day = scheduleMonday.Day;
                                obj.EndTime = scheduleMonday.EndTime.ToString();
                                obj.StartTime = scheduleMonday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Tuesday:
                            var scheduleTuesday = schedule.Where(s => s.Day.Equals("Tuesday")).FirstOrDefault();
                            if (scheduleTuesday == null || !scheduleTuesday.Day.Equals("Tuesday"))
                                obj.Day = "Tuesday";
                            else
                            {
                                obj.DoctorId = scheduleTuesday.DoctorId;
                                obj.Day = scheduleTuesday.Day;
                                obj.EndTime = scheduleTuesday.EndTime.ToString();
                                obj.StartTime = scheduleTuesday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Wednesday:
                            var scheduleWednesday = schedule.Where(s => s.Day.Equals("Wednesday")).FirstOrDefault();
                            if (scheduleWednesday == null || !scheduleWednesday.Day.Equals("Wednesday"))
                                obj.Day = "Wednesday";
                            else
                            {
                                obj.DoctorId = scheduleWednesday.DoctorId;
                                obj.Day = scheduleWednesday.Day;
                                obj.EndTime = scheduleWednesday.EndTime.ToString();
                                obj.StartTime = scheduleWednesday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Thursday:
                            var scheduleThursday = schedule.Where(s => s.Day.Equals("Thursday")).FirstOrDefault();
                            if (scheduleThursday == null || !scheduleThursday.Day.Equals("Thursday"))
                                obj.Day = "Thursday";
                            else
                            {
                                obj.DoctorId = scheduleThursday.DoctorId;
                                obj.Day = scheduleThursday.Day;
                                obj.EndTime = scheduleThursday.EndTime.ToString();
                                obj.StartTime = scheduleThursday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Friday:
                            var scheduleFriday = schedule.Where(s => s.Day.Equals("Friday")).FirstOrDefault();
                            if (scheduleFriday == null || !scheduleFriday.Day.Equals("Friday"))
                                obj.Day = "Friday";
                            else
                            {
                                obj.DoctorId = scheduleFriday.DoctorId;
                                obj.Day = scheduleFriday.Day;
                                obj.EndTime = scheduleFriday.EndTime.ToString();
                                obj.StartTime = scheduleFriday.StartTime.ToString();
                            }
                            break;
                        case DayOfWeek.Saturday:
                            var scheduleSaturday = schedule.Where(s => s.Day.Equals("Saturday")).FirstOrDefault();
                            if (scheduleSaturday == null || !scheduleSaturday.Day.Equals("Saturday"))
                                obj.Day = "Saturday";
                            else
                            {
                                obj.DoctorId = scheduleSaturday.DoctorId;
                                obj.Day = scheduleSaturday.Day;
                                obj.EndTime = scheduleSaturday.EndTime.ToString();
                                obj.StartTime = scheduleSaturday.StartTime.ToString();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    lst.Add(obj);
                }

            return View(lst);
            }
            catch { return null; }
        }


        [HttpPost]
        public ActionResult DoctorSchedules(FormCollection form)
        {
            try
            {
                if (!Request.IsAuthenticated)
                    return HttpNotFound();

                #region Sunday
                if (!string.IsNullOrWhiteSpace(form["sunday_start"].ToString()) &&
                    !string.IsNullOrWhiteSpace(form["sunday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId()).Where(s => s.Day.Equals("Sunday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Sunday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["sunday_start"].Trim().ToString();
                        model.EndTime = form["sunday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["sunday_start"].Trim().ToString();
                        schedule.EndTime = form["sunday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Monday

                if (!string.IsNullOrWhiteSpace(form["monday_start"].ToString()) &&
                   !string.IsNullOrWhiteSpace(form["monday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId()).Where(s => s.Day.Equals("Monday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Monday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["monday_start"].Trim().ToString();
                        model.EndTime = form["monday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["monday_start"].Trim().ToString();
                        schedule.EndTime = form["monday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Tuesday

                if (!string.IsNullOrWhiteSpace(form["tuesday_start"].ToString()) &&
                    !string.IsNullOrWhiteSpace(form["tuesday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId()).Where(s => s.Day.Equals("Tuesday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Tuesday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["tuesday_start"].Trim().ToString();
                        model.EndTime = form["tuesday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["tuesday_start"].Trim().ToString();
                        schedule.EndTime = form["tuesday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Wednesday

                //Wednesday
                if (!string.IsNullOrWhiteSpace(form["wednesday_start"].ToString()) &&
                   !string.IsNullOrWhiteSpace(form["wednesday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId())
                        .Where(s => s.Day.Equals("Wednesday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Wednesday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["wednesday_start"].Trim().ToString();
                        model.EndTime = form["wednesday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["wednesday_start"].Trim().ToString();
                        schedule.EndTime = form["wednesday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Thursday
                //Thursday
                if (!string.IsNullOrWhiteSpace(form["thursday_start"].ToString()) &&
                  !string.IsNullOrWhiteSpace(form["thursday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId())
                        .Where(s => s.Day.Equals("Thursday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Thursday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["thursday_start"].Trim().ToString();
                        model.EndTime = form["thursday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["thursday_start"].Trim().ToString();
                        schedule.EndTime = form["thursday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Friday
                //Friday
                if (!string.IsNullOrWhiteSpace(form["friday_start"].ToString()) &&
                 !string.IsNullOrWhiteSpace(form["friday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId())
                        .Where(s => s.Day.Equals("Friday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Friday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["friday_start"].Trim().ToString();
                        model.EndTime = form["friday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["friday_start"].Trim().ToString();
                        schedule.EndTime = form["friday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                #region Saturday
                //Saturday
                if (!string.IsNullOrWhiteSpace(form["saturday_start"].ToString()) &&
                !string.IsNullOrWhiteSpace(form["saturday_end"].ToString()))
                {
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId())
                        .Where(s => s.Day.Equals("Saturday")).FirstOrDefault();

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule();
                        model.Day = "Saturday";
                        model.DoctorId = User.Identity.GetUserId();
                        model.StartTime = form["saturday_start"].Trim().ToString();
                        model.EndTime = form["saturday_end"].Trim().ToString();
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["saturday_start"].Trim().ToString();
                        schedule.EndTime = form["saturday_end"].Trim().ToString();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }
                #endregion

                return RedirectToAction("DoctorSchedules");
            }
            catch { return null; }
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
                    var hpf = Request.Files[file];
                    if (hpf != null && hpf.ContentLength == 0)
                        continue;
                    if (hpf == null) continue;
                    prodId = Request.QueryString["pid"] + Path.GetExtension(hpf.FileName);
                    var folderPath = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    var savedFileName = Path.Combine(folderPath, prodId);
                    hpf.SaveAs(savedFileName);

                    r.Add(new UploadFilesResult
                    {
                        Name = hpf.FileName, Length = hpf.ContentLength, Type = hpf.ContentType
                    });
                }

                return Content("{\"name\":\"" + prodId + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + $"{r[0].Length} bytes" + "\"}", "application/json");
            }
            catch (Exception)
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