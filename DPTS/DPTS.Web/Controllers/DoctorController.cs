using DPTS.Web.Models;
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
using System.Xml.Linq;

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
        private readonly ApplicationDbContext _context;

        #endregion

        #region Contructor

        public DoctorController(IDoctorService doctorService, ISpecialityService specialityService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressService addressService, IAppointmentService scheduleService)
        {
            _doctorService = doctorService;
            _context = new ApplicationDbContext();
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
            return _context.Users.SingleOrDefault(u => u.Id == userId);
        }

        [NonAction]
        private static List<SelectListItem> GetGender()
        {
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem {Text = "Select Gender", Value = "0"}
            };
            items.AddRange(from object gender in Enum.GetValues(typeof (Gender))
                select new SelectListItem
                {
                    Text = Enum.GetName(typeof (Gender), gender),
                    Value = Enum.GetName(typeof (Gender), gender)
                });
            return items;
        }

        public IList<SelectListItem> GetAllSpecialities(string docterId)
        {
            var models = new List<SelectListItem>();
            var data = _specialityService.GetAllSpeciality(false);

            foreach (var speciality in data)
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
                        Selected = true
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
                select new {id = s.Id, name = s.Name})
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
                    result.Insert(0, new {id = 0, name = "None"});
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new {id = 0, name = "select state"});
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
                Text = type.Name,
                Value = type.Id.ToString()
            }));
            return typelst;
        }


        [NonAction]
        private static Dictionary<string, double> GetGeoCoordinate(string address)
        {
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            try
            {
                string requestUri = $"http://maps.google.com/maps/api/geocode/xml?address={address}&sensor=false";
                var request = System.Net.WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                var xElement = xdoc.Element(AppInfra.Constants.GeocodeResponse);
                var result = xElement?.Element(AppInfra.Constants.Result);
                var locationElement = result?.Element(AppInfra.Constants.Geometry)?.Element(AppInfra.Constants.Location);
                ParseLatLong(dictionary, locationElement);
            }
            catch (Exception ex)
            {
            }
            return dictionary;
        }

        private static void ParseLatLong(Dictionary<string, double> dictionary, XElement locationElement)
        {
            if (locationElement != null)
            {
                var lat = locationElement.Element(AppInfra.Constants.Lat);
                if (lat != null)
                    dictionary.Add(AppInfra.Constants.Lat, Double.Parse(lat.Value));
                var _long = locationElement.Element(AppInfra.Constants.Lng);
                if (_long != null)
                    dictionary.Add(AppInfra.Constants.Lng, Double.Parse(_long.Value));
            }
        }

        #endregion

        #region Methods

        public ActionResult ProfileSetting()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            var model = new DoctorProfileSettingViewModel();
            if (Request.IsAuthenticated && User.IsInRole("Doctor"))
            {
                var userId = User.Identity.GetUserId();
                var user = _context.Users.SingleOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var doctor = _doctorService.GetDoctorbyId(user.Id);
                    if (doctor != null)
                    {
                        model.DateCreated = doctor.DateCreated;
                        var dateOfBirth = string.IsNullOrWhiteSpace(doctor.DateOfBirth)
                            ? (DateTime?) null
                            : DateTime.Parse(doctor.DateOfBirth);
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
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

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

            var doctor = _doctorService.GetDoctorbyId(model.Id);
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

            return RedirectToAction("ProfileSetting");
        }

        public ActionResult AddressAdd()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            var model = new AddressViewModel {AvailableCountry = GetCountryList()};
            return View(model);
        }

        [HttpPost]
        public ActionResult AddressAdd(AddressViewModel model)
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = _context.Users.SingleOrDefault(u => u.Id == userId);

                var address = new Address
                {
                    StateProvinceId = model.StateProvinceId,
                    CountryId = model.CountryId,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    Hospital = model.Hospital,
                    FaxNumber = model.FaxNumber,
                    PhoneNumber = model.LandlineNumber,
                    Website = model.Website,
                    ZipPostalCode = model.ZipPostalCode,
                    City = model.City
                };
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                string state = address.StateProvinceId == 0
                    ? string.Empty
                    : _stateProvinceService.GetStateProvinceById(model.StateProvinceId).Name;
                string docAddress = model.Address1 + ", " + model.City + ", " + state + ", " + model.ZipPostalCode;
                var geoCoodrinate = GetGeoCoordinate(docAddress);
                if (geoCoodrinate.Count == 2)
                {
                    address.Latitude = geoCoodrinate[AppInfra.Constants.Lat];
                    address.Longitude = geoCoodrinate[AppInfra.Constants.Lng];
                }
                else
                {
                    var geoCoodrinates = GetGeoCoordinate(model.ZipPostalCode);
                    if (geoCoodrinates.Count == 2)
                    {
                        address.Latitude = geoCoodrinates[AppInfra.Constants.Lat];
                        address.Longitude = geoCoodrinates[AppInfra.Constants.Lng];
                    }
                }
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
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            var userId = User.Identity.GetUserId();
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new
                {
                    redirect = Url.Action("Addresses", "Doctor"),
                });

            var addrMapping = _addressService.GetAddressMappingbuUserIdAddrId(user.Id, addressId);

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
                redirect = Url.Action("Addresses", "Doctor"),
            });
        }

        public ActionResult Addresses()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            var userId = User.Identity.GetUserId();
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            var model = new List<AddressViewModel>();

            if (user != null)
                model.AddRange(_addressService.GetAllAddressByUser(user.Id).Select(c => new AddressViewModel
                {
                    Id = c.Id,
                    Address1 = c.Address1,
                    Address2 = c.Address2,
                    City = c.City,
                    CountryName =
                        c.CountryId.GetValueOrDefault() > 0
                            ? _countryService.GetCountryById(c.CountryId.GetValueOrDefault()).Name
                            : " ",
                    StateName =
                        c.StateProvinceId.GetValueOrDefault() > 0
                            ? _stateProvinceService.GetStateProvinceById(c.StateProvinceId.GetValueOrDefault()).Name
                            : " ",
                    FaxNumber = c.FaxNumber,
                    Hospital = c.Hospital,
                    LandlineNumber = c.PhoneNumber,
                    Website = c.Website,
                    ZipPostalCode = c.ZipPostalCode,
                }).ToList());

            return View(model);
        }

        public ActionResult AddressEdit(int id)
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            var address = _addressService.GetAddressbyId(id);
            if (address == null)
                return null;

            var model = new AddressViewModel
            {
                Id = address.Id,
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                CountryId = address.CountryId.GetValueOrDefault(),
                FaxNumber = address.FaxNumber,
                Hospital = address.Hospital,
                LandlineNumber = address.PhoneNumber,
                StateProvinceId = address.StateProvinceId.GetValueOrDefault(),
                Website = address.Website,
                ZipPostalCode = address.ZipPostalCode,
                AvailableCountry = GetCountryList(),
                //AvailableStateProvince= GetStatesByCountryId(address.CountryId,true)
            };

            model.AvailableCountry = GetCountryList();

            //states
            var states = _stateProvinceService
                .GetStateProvincesByCountryId(model.CountryId)
                .ToList();
            if (states.Any())
            {
                model.AvailableStateProvince.Add(new SelectListItem {Text = "Select state", Value = "0"});

                foreach (var s in states)
                {
                    model.AvailableStateProvince.Add(new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString(),
                        Selected = s.Id == model.StateProvinceId
                    });
                }
            }
            else
            {
                bool anyCountrySelected = model.AvailableCountry.Any(x => x.Selected);
                model.AvailableStateProvince.Add(new SelectListItem
                {
                    Text = anyCountrySelected ? "None" : "Select State",
                    Value = "0"
                });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(AddressViewModel model)
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
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

                string state = address.StateProvinceId == 0
                    ? string.Empty
                    : _stateProvinceService.GetStateProvinceById(model.StateProvinceId).Name;
                string docAddress = model.Address1 + ", " + model.City + ", " + state + ", " + model.ZipPostalCode;
                var geoCoodrinate = GetGeoCoordinate(docAddress);
                if (geoCoodrinate.Count == 2)
                {
                    address.Latitude = geoCoodrinate["lat"];
                    address.Longitude = geoCoodrinate["lng"];
                }
                else
                {
                    var geoCoodrinates = GetGeoCoordinate(model.ZipPostalCode);
                    if (geoCoodrinates.Count == 2)
                    {
                        address.Latitude = geoCoodrinates["lat"];
                        address.Longitude = geoCoodrinates["lng"];
                    }
                }

                _addressService.UpdateAddress(address);
                return RedirectToAction("Addresses");
            }
            //states

            model.AvailableCountry = GetCountryList();

            var states = _stateProvinceService
                .GetStateProvincesByCountryId(model.CountryId)
                .ToList();
            if (states.Any())
            {
                model.AvailableStateProvince.Add(new SelectListItem {Text = "Select state", Value = "0"});

                foreach (var s in states)
                {
                    model.AvailableStateProvince.Add(new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString(),
                        Selected = s.Id == model.StateProvinceId
                    });
                }
            }
            else
            {
                bool anyCountrySelected = model.AvailableCountry.Any(x => x.Selected);
                model.AvailableStateProvince.Add(new SelectListItem
                {
                    Text = anyCountrySelected ? "None" : "Select State",
                    Value = "0"
                });
            }
            return View(model);
        }

        public ActionResult Favourites()
        {
            return View();
        }

        public ActionResult InvoicesPackages()
        {
            return View();
        }

        public enum DayOfWeek
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
        }

        public ActionResult DoctorSchedules()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            try
            {
                var lst = new List<SheduleViewModel>();
                foreach (DayOfWeek day in Enum.GetValues(typeof (DayOfWeek)))
                {
                    var obj = new SheduleViewModel();
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId());
                    switch (day)
                    {
                        case DayOfWeek.Sunday:
                            var scheduleSunday = schedule.FirstOrDefault(s => s.Day.Equals("Sunday"));
                            if (scheduleSunday == null || !scheduleSunday.Day.Equals("Sunday"))
                                obj.Day = "Sunday";
                            else
                            {
                                obj.DoctorId = scheduleSunday.DoctorId;
                                obj.Day = scheduleSunday.Day;
                                obj.EndTime = scheduleSunday.EndTime;
                                obj.StartTime = scheduleSunday.StartTime;
                            }
                            break;
                        case DayOfWeek.Monday:
                            var scheduleMonday = schedule.FirstOrDefault(s => s.Day.Equals("Monday"));
                            if (scheduleMonday == null || !scheduleMonday.Day.Equals("Monday"))
                                obj.Day = "Monday";
                            else
                            {
                                obj.DoctorId = scheduleMonday.DoctorId;
                                obj.Day = scheduleMonday.Day;
                                obj.EndTime = scheduleMonday.EndTime;
                                obj.StartTime = scheduleMonday.StartTime;
                            }
                            break;
                        case DayOfWeek.Tuesday:
                            var scheduleTuesday = schedule.FirstOrDefault(s => s.Day.Equals("Tuesday"));
                            if (scheduleTuesday == null || !scheduleTuesday.Day.Equals("Tuesday"))
                                obj.Day = "Tuesday";
                            else
                            {
                                obj.DoctorId = scheduleTuesday.DoctorId;
                                obj.Day = scheduleTuesday.Day;
                                obj.EndTime = scheduleTuesday.EndTime;
                                obj.StartTime = scheduleTuesday.StartTime;
                            }
                            break;
                        case DayOfWeek.Wednesday:
                            var scheduleWednesday = schedule.FirstOrDefault(s => s.Day.Equals("Wednesday"));
                            if (scheduleWednesday == null || !scheduleWednesday.Day.Equals("Wednesday"))
                                obj.Day = "Wednesday";
                            else
                            {
                                obj.DoctorId = scheduleWednesday.DoctorId;
                                obj.Day = scheduleWednesday.Day;
                                obj.EndTime = scheduleWednesday.EndTime;
                                obj.StartTime = scheduleWednesday.StartTime;
                            }
                            break;
                        case DayOfWeek.Thursday:
                            var scheduleThursday = schedule.FirstOrDefault(s => s.Day.Equals("Thursday"));
                            if (scheduleThursday == null || !scheduleThursday.Day.Equals("Thursday"))
                                obj.Day = "Thursday";
                            else
                            {
                                obj.DoctorId = scheduleThursday.DoctorId;
                                obj.Day = scheduleThursday.Day;
                                obj.EndTime = scheduleThursday.EndTime;
                                obj.StartTime = scheduleThursday.StartTime;
                            }
                            break;
                        case DayOfWeek.Friday:
                            var scheduleFriday = schedule.FirstOrDefault(s => s.Day.Equals("Friday"));
                            if (scheduleFriday == null || !scheduleFriday.Day.Equals("Friday"))
                                obj.Day = "Friday";
                            else
                            {
                                obj.DoctorId = scheduleFriday.DoctorId;
                                obj.Day = scheduleFriday.Day;
                                obj.EndTime = scheduleFriday.EndTime;
                                obj.StartTime = scheduleFriday.StartTime;
                            }
                            break;
                        case DayOfWeek.Saturday:
                            var scheduleSaturday = schedule.FirstOrDefault(s => s.Day.Equals("Saturday"));
                            if (scheduleSaturday == null || !scheduleSaturday.Day.Equals("Saturday"))
                                obj.Day = "Saturday";
                            else
                            {
                                obj.DoctorId = scheduleSaturday.DoctorId;
                                obj.Day = scheduleSaturday.Day;
                                obj.EndTime = scheduleSaturday.EndTime;
                                obj.StartTime = scheduleSaturday.StartTime;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    lst.Add(obj);
                }

                return View(lst);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult DoctorSchedules(FormCollection form)
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            try
            {
                if (!Request.IsAuthenticated)
                    return HttpNotFound();

                #region Sunday

                GetScheduleByDay(form, "Sunday");

                #endregion

                #region Monday

                if (!string.IsNullOrWhiteSpace(form["monday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["monday_end"]))
                {
                    var schedule =
                        _scheduleService
                            .GetScheduleByDoctorId(User.Identity.GetUserId())
                            .FirstOrDefault(s => s.Day.Equals("Monday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Monday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["monday_start"].Trim(),
                            EndTime = form["monday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["monday_start"].Trim();
                        schedule.EndTime = form["monday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                #region Tuesday

                if (!string.IsNullOrWhiteSpace(form["tuesday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["tuesday_end"]))
                {
                    var schedule =
                        _scheduleService
                            .GetScheduleByDoctorId(User.Identity.GetUserId())
                            .FirstOrDefault(s => s.Day.Equals("Tuesday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Tuesday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["tuesday_start"].Trim(),
                            EndTime = form["tuesday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["tuesday_start"].Trim();
                        schedule.EndTime = form["tuesday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                #region Wednesday

                //Wednesday
                if (!string.IsNullOrWhiteSpace(form["wednesday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["wednesday_end"]))
                {
                    var schedule = _scheduleService
                        .GetScheduleByDoctorId(User.Identity.GetUserId()).FirstOrDefault(s => s.Day.Equals("Wednesday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Wednesday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["wednesday_start"].Trim(),
                            EndTime = form["wednesday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["wednesday_start"].Trim();
                        schedule.EndTime = form["wednesday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                #region Thursday

                //Thursday
                if (!string.IsNullOrWhiteSpace(form["thursday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["thursday_end"]))
                {
                    var schedule = _scheduleService
                        .GetScheduleByDoctorId(User.Identity.GetUserId()).FirstOrDefault(s => s.Day.Equals("Thursday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Thursday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["thursday_start"].Trim(),
                            EndTime = form["thursday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["thursday_start"].Trim();
                        schedule.EndTime = form["thursday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                #region Friday

                //Friday
                if (!string.IsNullOrWhiteSpace(form["friday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["friday_end"]))
                {
                    var schedule = _scheduleService
                        .GetScheduleByDoctorId(User.Identity.GetUserId()).FirstOrDefault(s => s.Day.Equals("Friday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Friday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["friday_start"].Trim(),
                            EndTime = form["friday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["friday_start"].Trim();
                        schedule.EndTime = form["friday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                #region Saturday

                //Saturday
                if (!string.IsNullOrWhiteSpace(form["saturday_start"]) &&
                    !string.IsNullOrWhiteSpace(form["saturday_end"]))
                {
                    var schedule = _scheduleService
                        .GetScheduleByDoctorId(User.Identity.GetUserId()).FirstOrDefault(s => s.Day.Equals("Saturday"));

                    if (schedule == null)
                    {
                        //insert record
                        var model = new Schedule
                        {
                            Day = "Saturday",
                            DoctorId = User.Identity.GetUserId(),
                            StartTime = form["saturday_start"].Trim(),
                            EndTime = form["saturday_end"].Trim()
                        };
                        _scheduleService.InsertSchedule(model);
                    }
                    else
                    {
                        //update record
                        schedule.StartTime = form["saturday_start"].Trim();
                        schedule.EndTime = form["saturday_end"].Trim();
                        _scheduleService.UpdateSchedule(schedule);
                    }
                }

                #endregion

                return RedirectToAction("DoctorSchedules");
            }
            catch
            {
                return null;
            }
        }

        private void GetScheduleByDay(FormCollection form, string day)
        {
            var lowerDay = day.ToLower();
            var lowerDayStart = day.ToLower()+ "_start";
            var lowerDayEnd = day.ToLower() + "_end";
            if (!string.IsNullOrWhiteSpace(form[lowerDayStart]) &&
                !string.IsNullOrWhiteSpace(form[lowerDayEnd]))
            {
                var schedule =
                    _scheduleService
                        .GetScheduleByDoctorId(User.Identity.GetUserId())
                        .FirstOrDefault(s => s.Day.Equals(day));

                if (schedule == null)
                {
                    var model = new Schedule
                    {
                        Day = lowerDay,
                        DoctorId = User.Identity.GetUserId(),
                        StartTime = form[lowerDayStart].Trim(),
                        EndTime = form[lowerDayEnd].Trim()
                    };
                    _scheduleService.InsertSchedule(model);
                }
                else
                {
                    schedule.StartTime = form[lowerDayStart].Trim();
                    schedule.EndTime = form[lowerDayEnd].Trim();
                    _scheduleService.UpdateSchedule(schedule);
                }
            }
        }

        public ActionResult BookingListings(DoctorScheduleListingViewModel model)
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            try
            {
                if (Request.IsAuthenticated && User.IsInRole("Doctor"))
                {
                    var userId = User.Identity.GetUserId();
                    var doctorSchedule = _scheduleService.GetAppointmentScheduleByDoctorId(userId);

                    if (!string.IsNullOrWhiteSpace(model.ByDate))
                    {
                        var sortedByDate = doctorSchedule.Where(s => s.AppointmentDate == model.ByDate).ToList();
                        model.AppointmentSchedule = sortedByDate;
                        return View(model);
                    }
                    model.AppointmentSchedule = doctorSchedule;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
        }

        public JsonResult ChangeBookingStatus(string type, string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(type))
                {
                    if (Request.IsAuthenticated && User.IsInRole("Doctor"))
                    {
                        var appoinment = _scheduleService.GetAppointmentScheduleById(int.Parse(id));
                        if (appoinment == null)
                            return Json(new {action_type = "none"});

                        AppointmentStatus status;
                        if (type.Equals("approve"))
                        {
                            status = _scheduleService.GetAppointmentStatusByName("Booked");
                            if (status == null)
                                return Json(new {action_type = "none"});

                            appoinment.StatusId = status.Id;
                            _scheduleService.UpdateAppointmentSchedule(appoinment);
                            return Json(new
                            {
                                action_type = "approved"
                            });
                        }
                        else if (type.Equals("cancel"))
                        {
                            status = _scheduleService.GetAppointmentStatusByName("Cancelled");
                            if (status == null)
                                return Json(new {action_type = "none"});

                            appoinment.StatusId = status.Id;
                            _scheduleService.UpdateAppointmentSchedule(appoinment);
                            return Json(new
                            {
                                action_type = "cancelled"
                            });
                        }
                        else if (type.Equals("visit"))
                        {
                            status = _scheduleService.GetAppointmentStatusByName("Visited");
                            if (status == null)
                                return Json(new {action_type = "none"});

                            appoinment.StatusId = status.Id;
                            _scheduleService.UpdateAppointmentSchedule(appoinment);
                            return Json(new
                            {
                                action_type = "visited"
                            });
                        }
                        else if (type.Equals("failed"))
                        {
                            status = _scheduleService.GetAppointmentStatusByName("Failed");
                            if (status == null)
                                return Json(new {action_type = "none"});

                            appoinment.StatusId = status.Id;
                            _scheduleService.UpdateAppointmentSchedule(appoinment);
                            return Json(new
                            {
                                action_type = "visited"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Todo Log it
            }
            return Json(new
            {
                action_type = "none"
            });
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
                var prodId = string.Empty;
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
                        Name = hpf.FileName,
                        Length = hpf.ContentLength,
                        Type = hpf.ContentType
                    });
                }

                return
                    Content(
                        "{\"name\":\"" + prodId + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" +
                        $"{r[0].Length} bytes" + "\"}", "application/json");
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
                model.Schedule = _scheduleService.GetScheduleByDoctorId(doctor.DoctorId);
                return View(model);
            }
            return View();
        }

        #endregion
    }
}