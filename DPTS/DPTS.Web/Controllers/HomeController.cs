using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Field
        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
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
        #endregion

        #region Constructor
        public HomeController(ISpecialityService specialityService,
            IDoctorService doctorService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
        }
        #endregion
        public ActionResult Index()
        {
            var model = new SearchModel();
            model.AvailableSpeciality = GetSpecialityList();
            return View(model);
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

            var data = _doctorService.SearchDoctor(model.keyword, model.SpecialityId);

            TempData["DoctorSearchResult"] = data.ToList();

            return RedirectToAction("SearchResult");
        }


        public ActionResult SearchResult()
        {
            IList<Doctor> doctor = (IList<Doctor>)TempData["DoctorSearchResult"];
            return View(doctor);
        }
    }
}