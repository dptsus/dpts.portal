using DPTS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPTS.Domain.Core.DefaultNotificationSettings;
using DPTS.Domain.Core.EmailCategory;
using DPTS.Domain.Entities;

namespace DPTS.Web.Controllers
{
    public class DefaultNotificationSettingsController : BaseController
    {
        #region Fields
        private readonly IDefaultNotificationSettingsService _defaultNotificationSettingsService;
        private readonly IEmailCategoryService _emailCategoryService;
        #endregion

        #region Contructor
        public DefaultNotificationSettingsController(IDefaultNotificationSettingsService stateProvinceService, IEmailCategoryService countryService)
        {
            _defaultNotificationSettingsService = stateProvinceService;
            _emailCategoryService = countryService;
        }
        #endregion

        #region Utilities

        public IList<SelectListItem> GetCountryList()
        {
            var countries = _emailCategoryService.GetAllEmailCategories(true);
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
        #endregion

        #region Methods
        public ActionResult List()
        {
            var countries = _defaultNotificationSettingsService.GetAllStateProvince(true);
            var model = countries.Select(c => new DefaultNotificationSettingsViewModel
            {
                Id = c.Id,
                Name = c.Name,
                CountryName = _emailCategoryService.GetEmailCategoryById(c.Id).Name,
                DisplayOrder = c.DisplayOrder,
                Abbreviation = c.Abbreviation,
                Published = c.Published,
                CountryId=c.CountryId
            }).ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            var model = new DefaultNotificationSettingsViewModel();
            model.AvailableCountry = GetCountryList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(DefaultNotificationSettingsViewModel model)
        {
            if (model.CountryId == 0)
                ModelState.AddModelError("", "select country");

            if (ModelState.IsValid)
            {
                var stateProvince = new DefaultNotificationSettings
                {
                    Name = model.Name,
                    Published = model.Published,
                    DisplayOrder = model.DisplayOrder,
                    Abbreviation = model.Abbreviation,
                    CountryId=model.CountryId
                };
                _defaultNotificationSettingsService.InsertStateProvince(stateProvince);
                return RedirectToAction("List");
            }
            model.AvailableCountry = GetCountryList();
            return View(model);
        }
        public ActionResult Edit(int Id)
        {
            if (!IsValidateId(Id))
                return HttpNotFound();

            var stateProvince = _defaultNotificationSettingsService.GetStateProvinceById(Id);
            if (stateProvince == null)
                return HttpNotFound();

            var model = new DefaultNotificationSettingsViewModel
            {
                Id = stateProvince.Id,
                Name = stateProvince.Name,
                CountryName=_emailCategoryService.GetEmailCategoryById(stateProvince.Id).Name,
                DisplayOrder = stateProvince.DisplayOrder,
                Published = stateProvince.Published,
                CountryId= stateProvince.CountryId,
                Abbreviation= stateProvince.Abbreviation,
                AvailableCountry = GetCountryList()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(DefaultNotificationSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var stateProvince = _defaultNotificationSettingsService.GetStateProvinceById(model.Id);
                stateProvince.Id = model.Id;
                stateProvince.Name = model.Name;
                stateProvince.DisplayOrder = model.DisplayOrder;
                stateProvince.Published = model.Published;
                stateProvince.CountryId = model.CountryId;
                stateProvince.Abbreviation = model.Abbreviation;

                _defaultNotificationSettingsService.UpdateStateProvince(stateProvince);
                return RedirectToAction("List");
            }
            model.AvailableCountry = GetCountryList();
            return View(model);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            var stateProvince = _defaultNotificationSettingsService.GetStateProvinceById(id);
            if (stateProvince != null)
                _defaultNotificationSettingsService.DeleteStateProvince(stateProvince);


            return Content("Deleted");
        }
        #endregion
    }
}