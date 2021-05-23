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
        public long DPS { get; set; }

        public long Damage { get; set; }

        public long CondiDPS { get; set; }

        public long CondiDamage { get; set; }

        public long PowerDPS { get; set; }

        public long PowerDamage { get; set; }

        public long ActorDPS { get; set; }

        public long ActorDamage { get; set; }

        public long ActorCondiDPS { get; set; }

        public long ActorCondiDamage { get; set; }

        public long ActorPowerDPS { get; set; }

        public long ActorPowerDamage { get; set; }

        public long PlayerID { get; set; }

        public LogPlayer Player { get; set; }
    }
}
