﻿using DPTS.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Speciality;
using System;
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
        public ActionResult Search(SearchModel model,int? page)
        {
            var pageNumber = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;

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

            var data = _doctorService.SearchDoctor(pageNumber, pageSize, out totalCount,
                model.geo_location,
                model.SpecialityId,
                model.geo_distance);

            var searchModel = new SearchModel
            {
                AvailableSpeciality = GetSpecialityList(),
                directory_type=model.directory_type,
                keyword=model.keyword,
                SpecialityId=model.SpecialityId,
                geo_location = model.geo_location
            };

            var searchViewModel = data.Select(doc => new TempDoctorViewModel
            {
                Doctors = doc, Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()
            }).ToList();

            totalCount = searchViewModel.Count;

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(searchViewModel, pageNumber + 1, pageSize, totalCount);
            ViewBag.SearchModel = searchModel;

            return View(pageDoctors);
        }


        /// <summary>
        /// Get all doctors
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Doctors(SearchModel model, int? page)
        {
            if (Request.QueryString.Count > 0 &&
                model.SpecialityId > 0 ||
                !string.IsNullOrWhiteSpace(model.geo_location) ||
                !string.IsNullOrWhiteSpace(model.keyword))
            {
                TempData["SearchModel"] = model;
                return RedirectToAction("Search");
            }

            var pageNumber = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;

            var data = _doctorService.GetAllDoctors(pageNumber, pageSize, out totalCount);

            var doctorViews = data.Select(doc => new TempDoctorViewModel
            {
                Doctors = doc, Address = _addressService.GetAllAddressByUser(doc.DoctorId).FirstOrDefault()
            }).ToList();

            IPagedList<TempDoctorViewModel> pageDoctors = new StaticPagedList<TempDoctorViewModel>(doctorViews, pageNumber + 1, pageSize, totalCount);

            var searchModel = new SearchModel
            {
                AvailableSpeciality = GetSpecialityList(),
            };
            ViewBag.SearchModel = searchModel;

            return View(pageDoctors);
        }
    }
}