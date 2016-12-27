using DPTS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        private ApplicationDbContext context;

        public DoctorController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Doctor
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ProfileSetting()
        {
            //var info=context.Users.fi
            return View();
        }
    }
}