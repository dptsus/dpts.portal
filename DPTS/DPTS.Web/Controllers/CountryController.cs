﻿using DPTS.Domain.Core;
using DPTS.Domain;
using DPTS.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class CountryController : BaseController
    {
        #region Fields
        private readonly ICountryService _countryService;
        #endregion

        #region Contructor
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }
        #endregion

        #region Methods
        public ActionResult List()
        {
            var countries = _countryService.GetAllCountries(true);
            var model = countries.Select(c => new CountryViewModel()
            {
                Id=c.Id,
                Name=c.Name,
                DisplayOrder=c.DisplayOrder,
                NumericIsoCode=c.NumericIsoCode,
                Published=c.Published,
                SubjectToVat=c.SubjectToVat,
                ThreeLetterIsoCode=c.ThreeLetterIsoCode,
                TwoLetterIsoCode=c.TwoLetterIsoCode

            }).ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            var model = new CountryViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(CountryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var country = new Country
                {
                    Name=model.Name,
                    TwoLetterIsoCode=model.TwoLetterIsoCode,
                    ThreeLetterIsoCode=model.ThreeLetterIsoCode,
                    NumericIsoCode=model.NumericIsoCode,
                    SubjectToVat=model.SubjectToVat,
                    Published=model.Published,
                    DisplayOrder=model.DisplayOrder
                };
                _countryService.InsertCountry(country);
                return RedirectToAction("List");
            }
            return View(model);
        }
        public ActionResult Edit(int Id)
        {
            if (!IsValidateId(Id))
                return HttpNotFound();

            var country = _countryService.GetCountryById(Id);
            if (country == null)
                return HttpNotFound();

            var model = new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                DisplayOrder = country.DisplayOrder,
                NumericIsoCode = country.NumericIsoCode,
                Published = country.Published,
                SubjectToVat = country.SubjectToVat,
                ThreeLetterIsoCode = country.ThreeLetterIsoCode,
                TwoLetterIsoCode = country.TwoLetterIsoCode
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(CountryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var country = _countryService.GetCountryById(model.Id);
                country.Id = model.Id;
                country.Name = model.Name;
                country.DisplayOrder = model.DisplayOrder;
                country.NumericIsoCode = model.NumericIsoCode;
                country.Published = model.Published;
                country.SubjectToVat = model.SubjectToVat;
                country.ThreeLetterIsoCode = model.ThreeLetterIsoCode;
                country.TwoLetterIsoCode = model.TwoLetterIsoCode;
                _countryService.UpdateCountry(country);
                return RedirectToAction("List");
            }
            return View();
        }


        public ActionResult DeleteConfirmed(int id)
        {
            var country = _countryService.GetCountryById(id);
            if (country != null)
                _countryService.DeleteCountry(country);


            return Content("Deleted");
        }
        #endregion
    }
}