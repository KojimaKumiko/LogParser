using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class ParsedLogFileDto
    {
        public ParsedLogFileDto()
        {
        }

        public ParsedLogFileDto(JObject logData)
        {
            if (logData == null)
            {
                throw new ArgumentNullException(nameof(logData));
            }

            EliteInsightsVersion = (string)logData["eliteInsightsVersion"];
            TriggerID = (long)logData["triggerID"];
            BossName = (string)logData["fightName"];
            BossIcon = (string)logData["fightIcon"];
            ArcVersion = (string)logData["arcVersion"];
            Gw2Build = (long)logData["gW2Build"];
            Language = (string)logData["language"];
            LanguageID = (long)logData["languageID"];
            RecordedBy = (string)logData["recordedBy"];
            StartTime = (DateTime)logData["timeStart"];
            EndTime = (DateTime)logData["timeEnd"];
            Duration = (string)logData["duration"];
            Success = (bool)logData["success"];
            IsCM = (bool)logData["isCM"];
        }

        public ParsedLogFileDto(JObject logData, List<LogPlayerDto> playerDtos) : this(logData)
        {
            Players = playerDtos ?? throw new ArgumentNullException(nameof(playerDtos));
        }

        public long ID { get; set; }

        public string EliteInsightsVersion { get; set; }

        public long TriggerID { get; set; }

        public string BossName { get; set; }

        public string BossIcon { get; set; }

        public string ArcVersion { get; set; }

        public long Gw2Build { get; set; }

        public string Language { get; set; }

        public long LanguageID { get; set; }

        public string RecordedBy { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Duration { get; set; }

        public bool Success { get; set; }

        public bool IsCM { get; set; }

        public string DpsReportLink { get; set; }

        public string HtmlPath { get; set; }

        public List<LogPlayerDto> Players { get; set; }
    }
}
