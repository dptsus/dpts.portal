using System.Diagnostics;

namespace KetanAgnihotriContosoPOS.Core.Models
{
    public class ExceptionEntry:EventEntry
    { 
        public string Message { get; set; }
        public string Severity => TraceEventType.Error.ToString();
        public string Priority => "Medium";
        public string StackStrace { get; set; }
        public string Exception { get; set; } 
    }
}
