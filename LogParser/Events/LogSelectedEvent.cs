using Database.Models;

namespace LogParser.Events
{
    public class LogSelectedEvent
    {
        public LogSelectedEvent(ParsedLogFile logFile)
        {
            LogFile = logFile;
        }
        
        public ParsedLogFile LogFile { get; set; }
    }
}
