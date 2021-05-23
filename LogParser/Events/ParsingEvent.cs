using System.Collections.Generic;
using LogParser.Models;

namespace LogParser.Events
{
    public class StartParsingEvent
    {
    }

    public class ParsingFinishedEvent
    {
        public ParsingFinishedEvent(List<ParsedLogFileDto> parsedLogFiles)
        {
            ParsedLogFiles = parsedLogFiles;
        }

        public List<ParsedLogFileDto> ParsedLogFiles { get; set; }
    }
}