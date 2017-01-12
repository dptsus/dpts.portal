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