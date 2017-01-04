using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using DPTS.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Field
        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
        private ApplicationUserManager _userManager;
        #endregion

        #region Constructor
        public HomeController(ISpecialityService specialityService,
            IDoctorService doctorService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
            UserManager = _userManager;
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


        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
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

            var searchTerms = model.keyword;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            var data = _doctorService.SearchDoctor(searchTerms,
                model.SpecialityId,
                model.directory_type);

            if (data == null)
                return null;

            var doctors = new List<DoctorViewModel>();
            foreach (var doc in data.ToList())
            {
                var _user = GetUserById(doc.DoctorId);
                if (_user.Result == null)
                    return null;

                var doctor = new DoctorViewModel
                {
                    Id=_user.Result.Id,
                    Email = _user.Result.Email,
                    FirstName = _user.Result.FirstName,
                    LastName = _user.Result.LastName,
                    MobileNumber = _user.Result.PhoneNumber,
                    doctor = doc
                };

                doctors.Add(doctor);
            }

            return View(doctors);
        }

    }
}