using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Models
{
    [Table("LogPlayer")]
    public class LogPlayer : BaseEntity
    {
        [JsonProperty("account")]
        public string AccountName { get; set; }

        [JsonProperty("group")]
        public long SubGroup { get; set; }
        
        [JsonProperty("hasCommanderTag")]
        public bool HasCommander { get; set; }
        
        [JsonProperty("profession")]
        public string Profession { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("condition")]
        public long Condition { get; set; }

        [JsonProperty("concentration")]
        public long Concentration { get; set; }

        [JsonProperty("healing")]
        public long Healing { get; set; }

        [JsonProperty("toughness")]
        public long Toughness { get; set; }

        [JsonProperty("instanceID")]
        public long Instance { get; set; }

        public ParsedLogFile ParsedLogFile { get; set; }

        public List<DpsTarget> DpsTargets { get; set; }
    }
}
