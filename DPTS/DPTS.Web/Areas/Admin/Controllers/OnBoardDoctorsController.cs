using DPTS.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Areas.Admin.Controllers
{
    public class OnBoardDoctorsController : Controller
    {
        // GET: Admin/OnBoardDoctors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/OnBoardDoctors/Create
        [HttpPost]
        public ActionResult Create(DoctorsRegistrationViewModel model)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }
    }
}
