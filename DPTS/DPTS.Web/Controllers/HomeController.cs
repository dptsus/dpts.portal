using DPTS.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Speciality;
using System;
using System.Security.Permissions;
using System.Xml.Linq;
using DPTS.Domain.Entities;
using PagedList;

namespace DPTS.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Field
        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
        private readonly IAddressService _addressService;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;
        #endregion

        #region Constructor
        public HomeController(ISpecialityService specialityService,
            IDoctorService doctorService,
            IAddressService addressService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
            UserManager = _userManager;
            context = new ApplicationDbContext();
            _addressService = addressService;
        }
        #endregion

        #region Utilities
        public IList<SelectListItem> GetSpecialityList()
        {
            var specialitys = _specialityService.GetAllSpeciality(false);
            List<SelectListItem> typelst = new List<SelectListItem>();
            typelst.Add(
                     new SelectListItem
                     {
                         Text = "Select Speciality",
                         Value = "0"
                     });
            foreach (var _type in specialitys.ToList())
            {
                typelst.Add(
                     new SelectListItem
                     {
                         Text = _type.Title,
                         Value = _type.Id.ToString()
                     });
            }
            return typelst;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ApplicationUser GetUserById(string userId)
        {
            return context.Users.SingleOrDefault(u => u.Id == userId);
        }
        #endregion

        public ActionResult Index()

        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            var model = new SearchModel
            {
                AvailableSpeciality = GetSpecialityList()
            };
            return PartialView(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
           ViewBag.Message = "Your contact page is here.";

            return View();
        }

        [ValidateInput(false)]
        public ActionResult Search(SearchModel model)
        {
            if (model == null)
                model = new SearchModel();

            if (TempData["SearchModel"] != null)
            {
                model = (SearchModel) TempData["SearchModel"];
            }


            var searchTerms = model.keyword;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            var data = _doctorService.SearchDoctor(model.geo_location);

           // var data = _doctorService.GetAllDoctors();

            var searchVmodel = new SearchViewModel();

            searchVmodel.SearchModel = new SearchModel
            {
                AvailableSpeciality = GetSpecialityList(),
                directory_type=model.directory_type,
                keyword=model.keyword,
                SpecialityId=model.SpecialityId,
                geo_location = model.geo_location
            };

            double lat = 0, lng = 0;
            string zipcode = model.geo_location;

            var firstZipCodeLocation = GetByZipCode(zipcode);

            if (firstZipCodeLocation == null)
            {
                firstZipCodeLocation = CalculateLatLngForZipCode(zipcode);
            }
            foreach (var doc in data)
            {
                var addr = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault();
                if (addr.Latitude == 0 && addr.Longitude == 0)
                {
                    var geoCoodrinateDealer = GetGeoCoordinate(addr.ZipPostalCode.Trim());
                    if (geoCoodrinateDealer.Count == 2)
                    {
                        lat = addr.Latitude = geoCoodrinateDealer["lat"];
                        lng = addr.Longitude = geoCoodrinateDealer["lng"];

                        _addressService.UpdateAddress(addr);
                    }
                }
                else
                {
                    lat = addr.Latitude;
                    lng = addr.Longitude;
                }

                if (firstZipCodeLocation != null && lat != 0 && lng != 0)
                {
                    var user = GetUserById(doc.DoctorId);
                    searchVmodel.doctorsModel.Add(new DoctorViewModel
                    {
                        doctor = doc,
                        Addresses = _addressService.GetAllAddressByUser(doc.DoctorId),
                        Email = user.Email,
                        FirstName =user.FirstName,
                        LastName = user.LastName,
                        MobileNumber = user.PhoneNumber,
                        Distance = CalculateDistance(firstZipCodeLocation.Latitude, firstZipCodeLocation.Longitude,
                            lat, lng),
                    });
                }
            }

            searchVmodel.doctorsModel = searchVmodel.doctorsModel.Where(c => c.Distance <= Convert.ToDouble(model.geo_distance)).OrderBy(c => c.Distance).ToList();

            if (model.SpecialityId > 0)
            {
                searchVmodel.doctorsModel = searchVmodel.doctorsModel.SelectMany(d => d.doctor.SpecialityMapping.Where(s => s.Speciality_Id.Equals(model.SpecialityId)), (d, s) => d).ToList();
            }

            return View(searchVmodel);
        }

        [NonAction]
        private IList<ZipCodes> GetAllZipCodes()
        {
            return _addressService.GetAllZipCodes();
        }

        [NonAction]
        private ZipCodes GetByZipCode(string zipCode)
        {
            var zipCodes = GetAllZipCodes();
            return zipCodes.FirstOrDefault(c => c.ZipCode == zipCode);

        }

        [NonAction]
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = CalculateDistanceByUnitType("Miles", dist);
            return dist;
        }

        [NonAction]
        private static double CalculateDistanceByUnitType(string unitType, double dist)
        {
            dist = dist * 60 * 1.1515;
            //if (unitType == DistanceUnitType.KiloMeters)
            //{
            //    dist *= 1.609344;
            //}
            return dist;
        }

        [NonAction]
        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        [NonAction]
        private static double Rad2Deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }

        [NonAction]
        private Dictionary<string, double> GetGeoCoordinate(string address)
        {
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            try
            {
                string requestUri = string.Format("http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=false", address);
                var request = System.Net.WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = xdoc.Element("GeocodeResponse").Element("result");
                if (result != null)
                {
                    var locationElement = result.Element("geometry").Element("location");
                    dictionary.Add("lat", Double.Parse(locationElement.Element("lat").Value));
                    dictionary.Add("lng", Double.Parse(locationElement.Element("lng").Value));
                }
            }
            catch (Exception ex)
            {
            }
            return dictionary;
        }

        [NonAction]
        private ZipCodes CalculateLatLngForZipCode(string zipcode)
        {
            var zipCodeLatLng = GetGeoCoordinate(zipcode);
            if (zipCodeLatLng.Count == 2)
            {
                //insert zipcode
                var zipCodeLocation = new ZipCodes
                {
                    ZipCode = zipcode,
                    Latitude = zipCodeLatLng["lat"],
                    Longitude = zipCodeLatLng["lng"]
                };

                _addressService
                    .InsertZipCode(zipCodeLocation);

                return zipCodeLocation;
            }

            return null;
        }

        /// <summary>
        /// Get all doctors
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Doctors(SearchModel model, int? page)
        {
            if (Request.QueryString.Count > 0)
            {
                TempData["SearchModel"] = model;
                return RedirectToAction("Search");
            }

            var pageNumber = (page ?? 1) - 1;
            var pageSize = 1;
            int totalCount; 

            var data = _doctorService.GetAllDoctors(pageNumber, pageSize, out totalCount);

            var doctorViews = data.Select(doc => new TempDoctorViewModel
            {
                Doctors = doc, Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()
            }).ToList();

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(doctorViews, pageNumber + 1, 1, totalCount);

            var searchModel = new SearchModel
            {
                AvailableSpeciality = GetSpecialityList(),
            };
            ViewBag.SearchModel = searchModel;

            return View(pageDoctors);
        }
    }
}