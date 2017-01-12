using DPTS.Domain.Core;
using DPTS.Domain;
using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class SubSpecialityController : BaseController
    {
        #region Field
        private readonly ISubSpecialityService _subSpecialityService;
        private readonly ISpecialityService _specialityService;
        #endregion

        #region Constructor
        public SubSpecialityController(ISubSpecialityService subSpecialityService, ISpecialityService specialityService)
        {
            _subSpecialityService = subSpecialityService;
            _specialityService = specialityService;
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
                         Text = "Select",
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

        #region Method
        public ActionResult List()
        {
            var model = _subSpecialityService.GetAllSubSpeciality(false).Select(c => new SubSpecialityViewModel
            {
                Name=c.Name,
                Id=c.Id,
                IsActive=c.IsActive,
                SpecialityId=c.SpecialityId,
                SpecialityName= _specialityService.GetSpecialitybyId(c.SpecialityId).Title,
                DisplayOrder=c.DisplayOrder
            });
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new SubSpecialityViewModel();
            model.AvailableSpeciality = GetSpecialityList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SubSpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subSpeciality = new SubSpeciality
                {
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    SpecialityId=model.SpecialityId
                };
                _subSpecialityService.AddSubSpeciality(subSpeciality);
                return RedirectToAction("List");
            }
            model.AvailableSpeciality = GetSpecialityList();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!IsValidateId(id))
                return null;

            var subSpeciality = _subSpecialityService.GetSubSpecialitybyId(id);
            if (subSpeciality == null)
                return null;

            var model = new SubSpecialityViewModel
            {
                Id = subSpeciality.Id,
                Name = subSpeciality.Name,
                SpecialityName=_specialityService.GetSpecialitybyId(subSpeciality.SpecialityId).Title,
                IsActive = subSpeciality.IsActive,
                DisplayOrder = subSpeciality.DisplayOrder,
                SpecialityId=subSpeciality.SpecialityId,
                DateUpdated=subSpeciality.DateCreated,
                DateCreated=subSpeciality.DateCreated,
                AvailableSpeciality = GetSpecialityList()

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SubSpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subSpeciality = _subSpecialityService.GetSubSpecialitybyId(model.Id);
                subSpeciality.Name = model.Name;
                subSpeciality.DisplayOrder = model.DisplayOrder;
                subSpeciality.IsActive = model.IsActive;
                subSpeciality.SpecialityId = model.SpecialityId;
                _subSpecialityService.UpdateSubSpeciality(subSpeciality);
                return RedirectToAction("List");
            }
            return View(model);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            var subSpeciality = _subSpecialityService.GetSubSpecialitybyId(id);
            if (subSpeciality != null)
                _subSpecialityService.DeleteSubSpeciality(subSpeciality);


            return Content("Deleted");
        }
        #endregion
    }
}