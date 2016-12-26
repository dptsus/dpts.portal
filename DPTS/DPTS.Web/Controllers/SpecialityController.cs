using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class SpecialityController : Controller
    {
        #region Fields
        private readonly ISpecialityService _specialityService;
        #endregion

        #region Constructor
        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }
        #endregion

        #region utitlity
        [NonAction]
        protected bool IsValidateId(int id)
        {
            return id != 0;
        }
        #endregion
        // GET: Speciality
        #region Methods
        public async Task<ActionResult> List()
        {
           var model = await _specialityService.GetAllSpecialityAsync(true);
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Speciality();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Speciality model)
        {
            if (ModelState.IsValid)
            {
                var speciality = new Speciality
                {
                    Title = model.Title,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    DateCreated=DateTime.UtcNow,
                    DateUpdated=DateTime.UtcNow
                };
                _specialityService.AddSpecialityAsync(speciality);
                return RedirectToAction("List");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!IsValidateId(id))
                return null;

            var speciality = await _specialityService.GetSpecialitybyIdAsync(id);
            if (speciality == null)
                return null;

            var model = new Speciality
            {
                Id = speciality.Id,
                Title = speciality.Title,
                IsActive = speciality.IsActive,
                DisplayOrder = speciality.DisplayOrder,
                DateUpdated=speciality.DateUpdated
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Speciality model)
        {
            if (ModelState.IsValid)
            {
                var speciality = await _specialityService.GetSpecialitybyIdAsync(model.Id);
                speciality.Title = model.Title;
                speciality.DisplayOrder = model.DisplayOrder;
                speciality.IsActive = model.IsActive;
                speciality.DateCreated = DateTime.UtcNow;
                speciality.DateUpdated = DateTime.UtcNow;
                _specialityService.UpdateSpecialityAsync(speciality);
                return RedirectToAction("List");
            }
            return View(model);
        }
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var speciality = await _specialityService.GetSpecialitybyIdAsync(id);
            if (speciality != null)
               await _specialityService.DeleteSpecialityAsync(speciality);

            return Content("Deleted");
        }
        #endregion

    }
}