using System.Collections.Generic;
using Database.Models;

namespace LogParser.Events
{
    public class StartParsingEvent
    {
    }

    public class ParsingFinishedEvent
    {
        public ParsingFinishedEvent(List<ParsedLogFile> parsedLogFiles)
        {
            ParsedLogFiles = parsedLogFiles;
        }

        public List<ParsedLogFile> ParsedLogFiles { get; set; }
    }
}