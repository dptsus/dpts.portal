using KetanAgnihotriContosoPOS.Core.Models;

namespace KetanAgnihotriContosoPOS.Core.Contracts
{
    public interface ILogManager
    {
        void LogApplicationCalls(EventEntry eventLogEntry); 
    }
}
