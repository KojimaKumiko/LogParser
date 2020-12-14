using Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public interface IDiscordService
    {
        public Task SendReports(IList<ParsedLogFile> logFiles);
    }
}
