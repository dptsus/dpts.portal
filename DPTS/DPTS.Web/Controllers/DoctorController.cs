using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Customer
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ProfileSetting()
        {
            return View();
        }
    }
}