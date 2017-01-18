//using System;
//using System.Diagnostics;
//using System.Web.Mvc;
//using KetanAgnihotriContosoPOS.Core;
//using KetanAgnihotriContosoPOS.Core.Contracts;
//using KetanAgnihotriContosoPOS.Core.Models;

//namespace KetanAgnihotriContosoPOS.Filters
//{
//    public class LogApplicationAttribute : ActionFilterAttribute
//    {
//        private Stopwatch _stopwatch;

//        private readonly ILogManager _fileLogger;
//        public LogApplicationAttribute()
//        {
//            _fileLogger = new FileLogManager();
//        }

//        public override void OnActionExecuting(ActionExecutingContext actionContext)
//        {
//            _stopwatch = new Stopwatch();
//            base.OnActionExecuting(actionContext);
//        }

//        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
//        {
//            var eventEntry = GetEventEntry(actionExecutedContext); 
//            _fileLogger.LogApplicationCalls(eventEntry);
//           // base.OnActionExecuted(actionExecutedContext);
//        }
//        private EventEntry GetEventEntry(ActionExecutedContext actionExecutedContext)
//        {
//            return new EventEntry
//            {
//                Title = Core.AppInfra.Constants.ApplicationName + "-" + actionExecutedContext.ActionDescriptor.ControllerDescriptor.ControllerName,
//                CallType = actionExecutedContext.ActionDescriptor.ActionName,
//                ResponseTime = Convert.ToDecimal(_stopwatch.Elapsed.TotalSeconds),
//                Status = "True",  
//            };
//        }

//    }
//}