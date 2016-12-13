using System;
using System.Configuration;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace dpts.portal.Controllers
{
    public class BaseController : Controller
    {
        public class RolesAttribute : AuthorizeAttribute
        {
            public RolesAttribute(params string[] roles)
            {
                Roles = string.Join(",", roles);
            }
        }


        [HttpPost]
        public bool ClearSession()
        {
            try
            {
                Session.Clear();
                return true;
            }
            catch (Exception e)
            {
                // ExceptionHandler.HandleException(e);
                return false;
            }
        }
    }


    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        private int Duration { get; set; }

        public CacheFilterAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Duration <= 0) return;

            var cache = filterContext.HttpContext.Response.Cache;
            var cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }

    public class CompressFilter : ActionFilterAttribute
    {
        //FilterExecutingContext  
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            var acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            var response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["MinificationEnabled"] == "true")
            {
                filterContext.Controller.ViewBag.MinificationEnabled = ".min";
            }
            else
            {
                filterContext.Controller.ViewBag.MinificationEnabled = "";
            }
        }
    }
}