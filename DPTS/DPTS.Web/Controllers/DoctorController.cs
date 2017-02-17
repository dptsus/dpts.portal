using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Appointment;
using DPTS.Domain.Core.Country;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Speciality;
using DPTS.Domain.Core.StateProvince;
using DPTS.Domain.Entities;
using DPTS.Web.Models;
using Microsoft.AspNet.Identity;
using Kendo.Mvc.UI;
using HttpVerbs = System.Web.Mvc.HttpVerbs;
using DPTS.Services;

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

        private static void GetDoctorScheduleByDay(SheduleViewModel obj, IList<Schedule> schedule, string day)
        {
            var scheduleSunday = schedule?.FirstOrDefault(s => s.Day.Equals(day));
            if (scheduleSunday == null || !scheduleSunday.Day.Equals(day))
                obj.Day = day;
            else
            {
                obj.DoctorId = scheduleSunday.DoctorId;
                obj.Day = scheduleSunday.Day;
                obj.EndTime = scheduleSunday.EndTime;
                obj.StartTime = scheduleSunday.StartTime;
            }
        }

        private void GetScheduleByDay(FormCollection form, string day)
        {
            if (!string.IsNullOrWhiteSpace(day))
            {
                var lowerDay = day.ToLower();
                var lowerDayStart = day.ToLower() + "_start";
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
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
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
            doctor.DateUpdated = DateTime.UtcNow;
            doctor.Qualifications = model.Qualifications;
            doctor.RegistrationNumber = model.RegistrationNumber;
            doctor.YearsOfExperience = model.NoOfYearExperience;
            _doctorService.UpdateDoctor(doctor);

            return RedirectToAction("ProfileSetting");
        }

        #region Address

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

        #endregion

        #region Favourites & Invoices
        public ActionResult Favourites()
        {
            return View();
        }

        public ActionResult InvoicesPackages()
        {
            return View();
        }
        #endregion

        #region Schedule
        public ActionResult DoctorSchedules()
        {
            if (!Request.IsAuthenticated && !User.IsInRole("Doctor"))
                return new HttpUnauthorizedResult();

            try
            {
                var lst = new List<SheduleViewModel>();
                foreach (AppInfra.DayOfWeek day in Enum.GetValues(typeof (DayOfWeek)))
                {
                    var obj = new SheduleViewModel();
                    var schedule = _scheduleService.GetScheduleByDoctorId(User.Identity.GetUserId());
                    switch (day)
                    {
                        case AppInfra.DayOfWeek.Sunday:
                            GetDoctorScheduleByDay(obj, schedule, "Sunday");
                            break;
                        case AppInfra.DayOfWeek.Monday:
                            GetDoctorScheduleByDay(obj, schedule, "Monday");
                            break;
                        case AppInfra.DayOfWeek.Tuesday:
                            GetDoctorScheduleByDay(obj, schedule, "Tuesday");
                            break;
                        case AppInfra.DayOfWeek.Wednesday:
                            GetDoctorScheduleByDay(obj, schedule, "Wednesday");
                            break;
                        case AppInfra.DayOfWeek.Thursday:
                            GetDoctorScheduleByDay(obj, schedule, "Thursday");
                            break;
                        case AppInfra.DayOfWeek.Friday:
                            GetDoctorScheduleByDay(obj, schedule, "Friday");
                            break;
                        case AppInfra.DayOfWeek.Saturday:
                            GetDoctorScheduleByDay(obj, schedule, "Saturday");
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

                GetScheduleByDay(form, "Sunday");
                GetScheduleByDay(form, "Monday");
                GetScheduleByDay(form, "Tuesday");
                GetScheduleByDay(form, "Wednesday");
                GetScheduleByDay(form, "Thursday");
                GetScheduleByDay(form, "Friday");
                GetScheduleByDay(form, "Saturday");

                return RedirectToAction("DoctorSchedules");
            }
            catch (Exception e)
            {
                return null; //Todo Log it
            }
        }

        #endregion

        #region Booking

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

        public ActionResult BookingSettings()
        {
            return View();
        }

        #endregion

        #region Security & Privacy
        public ActionResult SecuritySettings()
        {
            return View();
        }

        public ActionResult PrivacySettings()
        {
            return View();
        }
        #endregion

        #region Profile Image
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
        #endregion

        #region Details
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

        #region Social Links

        [HttpPost]
        public ActionResult SocialLink_Read(DataSourceRequest command, string docterId)
        {
            var socialSites = _doctorService.GetAllLinksByDoctor(docterId, command.Page - 1, 5, false);
            var gridModel = new DataSourceResult
            {
                Data = socialSites.Select(x => new SocialLinkInformation
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    DoctorId = x.DoctorId,
                    SocialLink = x.SocialLink,
                    SocialType = x.SocialType
                }),
                Total = socialSites.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult SocialLink_Add([Bind(Exclude = "Id")] SocialLinkInformation model, string docterId)
        {
            if (model.SocialType != null)
                model.SocialType = model.SocialType.Trim();
            if (model.SocialLink != null)
                model.SocialLink = model.SocialLink.Trim();
            if (docterId != null)
                model.DoctorId = docterId;

            if (!ModelState.IsValid && model.DoctorId == null)
            {
                return Json(new DataSourceResult {Errors = "error"});
            }

            _doctorService.InsertSocialLink(model);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult SocialLink_Update(SocialLinkInformation model, string docterId)
        {
            var link = _doctorService.GetSocialLinkbyId(model.Id);
            if (link == null)
                return Content("No link could be loaded with the specified ID");

            if (!link.SocialType.Equals(model.SocialType, StringComparison.InvariantCultureIgnoreCase) ||
                link.Id != model.Id)
            {
                _doctorService.DeleteSocialLink(link);
            }

            link.Id = model.Id;
            link.DoctorId = model.DoctorId;
            link.SocialType = model.SocialType;
            link.SocialLink = model.SocialLink;
            link.IsActive = model.IsActive;
            link.DisplayOrder = model.DisplayOrder;
            _doctorService.UpdateSocialLink(link);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult SocialLink_Delete(int id)
        {
            var link = _doctorService.GetSocialLinkbyId(id);
            if (link == null)
                throw new ArgumentException("No link found with the specified id");
            _doctorService.DeleteSocialLink(link);

            return new NullJsonResult();
        }

        #endregion

        #region Honors & Awards
        [HttpPost]
        public ActionResult HonorsAwards_Read(DataSourceRequest command, string docterId)
        {
            var awardsHonars = _doctorService.GetAllHonorsAwards(docterId, command.Page - 1, 5, false);
            var gridModel = new DataSourceResult
            {
                Data = awardsHonars.Select(x => new HonorsAwards
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    DoctorId = x.DoctorId,
                    Name = x.Name,
                    Description = x.Description,
                    AwardDate = x.AwardDate
                }),
                Total = awardsHonars.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult HonorsAwards_Add([Bind(Exclude = "Id")] HonorsAwards awards, string docterId)
        {
            if (awards.Name != null)
                awards.Name = awards.Name.Trim();
            if (awards.Description != null)
                awards.Description = awards.Description.Trim();
            if (docterId != null)
                awards.DoctorId = docterId;

            if (!ModelState.IsValid && awards.DoctorId == null)
            {
                return Json(new DataSourceResult { Errors = "error" });
            }

            _doctorService.InsertHonorsAwards(awards);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult HonorsAwards_Update(HonorsAwards awards, string docterId)
        {
            var award = _doctorService.GetHonorsAwardsbyId(awards.Id);
            if (award == null)
                return Content("No link could be loaded with the specified ID");

            if (!award.Name.Equals(awards.Name, StringComparison.InvariantCultureIgnoreCase) ||
                award.Id != awards.Id)
            {
                _doctorService.DeleteHonorsAwards(award);
            }

            award.Id = awards.Id;
            award.DoctorId = awards.DoctorId;
            award.Name = awards.Name;
            award.Description = awards.Description;
            award.AwardDate = awards.AwardDate;
            award.IsActive = awards.IsActive;
            award.DisplayOrder = awards.DisplayOrder;
            _doctorService.UpdateHonorsAwards(award);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult HonorsAwards_Delete(HonorsAwards awards)
        {
            if (awards.Id > 0)
            {
                var award = _doctorService.GetHonorsAwardsbyId(awards.Id);
                if (award == null)
                    throw new ArgumentException("No Awards found with the specified id");
                _doctorService.DeleteHonorsAwards(award);
            }

            return new NullJsonResult();
        }
        #endregion

        #region Education
        [HttpPost]
        public ActionResult Education_Read(DataSourceRequest command, string docterId)
        {
            var education = _doctorService.GetAllEducation(docterId, command.Page - 1, 5, false);
            var gridModel = new DataSourceResult
            {
                Data = education.Select(x => new Education
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    DoctorId = x.DoctorId,
                    Title = x.Title,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Institute = x.Institute
                }),
                Total = education.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult Education_Add([Bind(Exclude = "Id")] Education education, string docterId)
        {
            if (education.Title != null)
                education.Title = education.Title.Trim();
            if (education.Description != null)
                education.Description = education.Description.Trim();
            if (education.Institute != null)
                education.Institute = education.Institute.Trim();
            if (docterId != null)
                education.DoctorId = docterId;

            if (!ModelState.IsValid && education.DoctorId == null)
            {
                return Json(new DataSourceResult { Errors = "error" });
            }

            _doctorService.InsertEducation(education);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult Education_Update(Education education, string docterId)
        {
            var edu = _doctorService.GetEducationbyId(education.Id);
            if (edu == null)
                return Content("No education could be loaded with the specified ID");

            if (!education.Title.Equals(edu.Title, StringComparison.InvariantCultureIgnoreCase) ||
                education.Id != edu.Id)
            {
                _doctorService.DeleteEducation(edu);
            }

            edu.Id = education.Id;
            edu.DoctorId = education.DoctorId;
            edu.Title = education.Title;
            edu.Description = education.Description;
            edu.IsActive = education.IsActive;
            edu.DisplayOrder = education.DisplayOrder;
            edu.Institute = education.Institute;
            edu.StartDate = education.StartDate;
            edu.EndDate = education.EndDate;
            _doctorService.UpdateEducation(edu);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult Education_Delete(Education education)
        {
            if (education.Id > 0)
            {
                var edu = _doctorService.GetEducationbyId(education.Id);
                if (edu == null)
                    throw new ArgumentException("No education found with the specified id");
                _doctorService.DeleteEducation(edu);
            }

            return new NullJsonResult();
        }
        #endregion

        #endregion
    }
}