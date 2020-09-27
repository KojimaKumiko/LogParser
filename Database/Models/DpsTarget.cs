using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Models
{
    [Table("DpsTarget")]
    public class DpsTarget : BaseEntity
    {
        [JsonProperty("dps")]
        public long DPS { get; set; }

        [JsonProperty("damage")]
        public long Damage { get; set; }

        [JsonProperty("condiDps")]
        public long CondiDPS { get; set; }

        [JsonProperty("condiDamage")]
        public long CondiDamage { get; set; }

        [JsonProperty("powerDps")]
        public long PowerDPS { get; set; }

        [JsonProperty("powerDamage")]
        public long PowerDamage { get; set; }

        [JsonProperty("actorDps")]
        public long ActorDPS { get; set; }

        [JsonProperty("actorDamage")]
        public long ActorDamage { get; set; }

        [JsonProperty("actorCondiDps")]
        public long ActorCondiDPS { get; set; }

        [JsonProperty("actorCondiDamage")]
        public long ActorCondiDamage { get; set; }

        [JsonProperty("actorPowerDps")]
        public long ActorPowerDPS { get; set; }

        [JsonProperty("actorPowerDamage")]
        public long ActorPowerDamage { get; set; }

        public long PlayerID { get; set; }

        public LogPlayer Player { get; set; }
    }
}
