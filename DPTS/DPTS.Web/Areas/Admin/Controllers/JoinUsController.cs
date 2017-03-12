using DPTS.Domain.Core.Doctors;
using DPTS.Web.Areas.Admin.Models;
using DPTS.Web.Models;
using reCaptcha;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Areas.Admin.Controllers
{
    public class JoinUsController : Controller
    {

        private readonly IJoinUsService _doctorService;
        public JoinUsController(IJoinUsService doctorService)
        {
            _doctorService = doctorService;
        }
        public ActionResult Start()
        {
            ViewBag.RecaptchaLastErrors = ReCaptcha.GetLastErrors(this.HttpContext);
            ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

            return View(new JoinUsViewModel());
        }
        public ActionResult Thanks()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(JoinUsViewModel model)
        {
            try
            {
                if (true)
                {

                    return RedirectToAction("Thanks");
                }
                return RedirectToAction("Error");
            }
            catch
            {
                return View();
            }
        }
    }
}
