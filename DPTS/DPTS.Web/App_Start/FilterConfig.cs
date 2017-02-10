using System.Web.Mvc;
using DPTS.Web.AppFilters;
using DPTS.Web.Controllers;

namespace DPTS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionLogingAttribute());
            filters.Add(new LogApplicationAttribute());
            //filters.Add(new CompressFilter()); 
        }
    }
}