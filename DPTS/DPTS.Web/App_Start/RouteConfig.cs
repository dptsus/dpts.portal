using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DPTS.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );
            //get state list by country ID  (AJAX link)
            routes.MapRoute("GetStatesByCountryId",
                            "Doctor/GetStatesByCountryId/",
                            new { controller = "Doctor", action = "GetStatesByCountryId" }
                           );
            routes.MapRoute("DPTSSearch",
                         "search/",
                         new { controller = "Home", action = "Search" });
            routes.MapRoute(
                "BookAppoinment",
                "appoinment/{doctorId}",
                new { controller = "Appointment", action = "AppointmentSchedule", doctorId = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    "ContactUs",
            //    "contact",
            //    new { controller = "Home", action = "Contact" }
            //);
            //routes.MapRoute(
            //    "AboutUs",
            //    "about",
            //    new { controller = "Home", action = "About" }
            //);
            //routes.MapRoute(
            //    "AdminDashbrd",
            //    "admin",
            //    new { controller = "Administration", action = "Index" }
            //);
            //routes.MapRoute(
            //    "Register",
            //    "register",
            //    new { controller = "Account", action = "Register" }
            //);
            //routes.MapRoute(
            //    "Login",
            //    "login",
            //    new { controller = "Account", action = "Login" }
            //);
            //routes.MapRoute(
            //    "AccountManage",
            //    "manage",
            //    new { controller = "Manage", action = "index" }
            //);

        }
    }
}
