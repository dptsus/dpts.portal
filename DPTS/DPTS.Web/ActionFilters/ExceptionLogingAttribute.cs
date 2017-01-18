//using System.Web.Mvc;
//using KetanAgnihotriContosoPOS.Core;
//using KetanAgnihotriContosoPOS.Core.Contracts;
//using KetanAgnihotriContosoPOS.Core.Models;

//namespace KetanAgnihotriContosoPOS.Filters
//{
//    public class ExceptionLogingAttribute : IExceptionFilter
//    {
//        private readonly ILogManager _fileLogger;
//        public ExceptionLogingAttribute()
//        {
//            _fileLogger = new FileLogManager();
//        }
//        public void OnException(ExceptionContext filterContext)
//        { 
//            _fileLogger.LogApplicationCalls(GetExceptionEntry(filterContext));
//        }

//        private static ExceptionEntry GetExceptionEntry(ExceptionContext filterContext)
//        {
//            return new ExceptionEntry
//            {
//                Title = Core.AppInfra.Constants.ApplicationName,
//                Message = filterContext?.Exception?.Message,
//                Exception = filterContext?.Exception?.ToString(),
//                StackStrace = filterContext?.Exception?.StackTrace
//            };
//        }
//    }
//}