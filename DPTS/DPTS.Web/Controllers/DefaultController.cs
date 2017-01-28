using System;
using System.Net;
using System.Web.Http;

namespace DPTS.Web.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("HealthCheck")]
        public IHttpActionResult HealthCheck()
        { 
            try
            { 
                return Content(HttpStatusCode.OK, "HealthCheck Successful");
            }
            catch (Exception ex)
            { 
                return Content(HttpStatusCode.NotImplemented, "HealthCheck Unsuccessful");
            }
        }
    }
}
