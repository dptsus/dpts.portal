using SQS_Shopee.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class AdministrationController : Controller
    {
        // GET: Administration
        [Route("/admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}