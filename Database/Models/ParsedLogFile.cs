using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    [Table("ParsedLogFile")]
    public class ParsedLogFile : BaseEntity
    {
        [JsonProperty("eliteInsightsVersion")]
        public string EliteInsightsVersion { get; set; }

        [JsonProperty("triggerID")]
        public long TriggerID { get; set; }

        [JsonProperty("fightName")]
        public string BossName { get; set; }

        [JsonProperty("fightIcon")]
        public string BossIcon { get; set; }

        [JsonProperty("arcVersion")]
        public string ArcVersion { get; set; }

        [JsonProperty("gW2Build")]
        public long Gw2Build { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("languageID")]
        public long LanguageID { get; set; }

        [JsonProperty("recordedBy")]
        public string RecordedBy { get; set; }

        [JsonProperty("timeStartStd")]
        public DateTime StartTime { get; set; }

        [JsonProperty("timeEndStd")]
        public DateTime EndTime { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("isCM")]
        public bool IsCM { get; set; }

        public string DpsReportLink { get; set; }

        public string HtmlPath { get; set; }

        public List<LogPlayer> Players { get; set; }
    }
}
