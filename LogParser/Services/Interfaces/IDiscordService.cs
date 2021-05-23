using Database.Models;
using LogParser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public interface IDiscordService
    {
        public Task SendReports(IList<ParsedLogFileDto> logFiles);
    }
}
