using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        #region Fields
        private IDoctorService _doctorService;
        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        #endregion

        #region Utilities
        [NonAction]
        private List<SelectListItem> GetGender()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Select Gender", Value = "0" });
            foreach (var gender in Enum.GetValues(typeof(Gender)))
            {
                items.Add(new SelectListItem()
                {
                    Text = Enum.GetName(typeof(Gender), gender),
                    Value = Enum.GetName(typeof(Gender), gender)
                });
            }
            return items;
        }
        #endregion

        #region Methods
        public async Task<ActionResult> Info()
        {
            var data = await _doctorService.GetAllDoctorAsync(showhidden: true, enableTracking: true);
            return View();
        }

        public ActionResult ProfileSetting()
        {
            ViewBag.GenderList = GetGender();
            return View();
        }
        #endregion

    }
}