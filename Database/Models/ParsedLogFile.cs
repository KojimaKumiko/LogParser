using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Models
{
    [Table("ParsedLogFile")]
    public class ParsedLogFile : BaseEntity
    {
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

        public DateTime Duration { get; set; }

        public bool Success { get; set; }

        public bool IsCM { get; set; }

        public List<LogPlayer> Players { get; set; }
    }
}
