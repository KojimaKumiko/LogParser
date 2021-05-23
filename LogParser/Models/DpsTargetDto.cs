using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class DpsTargetDto
    {
        public DpsTargetDto()
        {
        }

        public DpsTargetDto(JObject jsonData)
        {
            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            DPS = (long)jsonData["dps"];
            Damage = (long)jsonData["damage"];
            CondiDPS = (long)jsonData["condiDps"];
            CondiDamage = (long)jsonData["condiDamage"];
            PowerDPS = (long)jsonData["powerDps"];
            PowerDamage = (long)jsonData["powerDamage"];
            ActorDPS = (long)jsonData["actorDps"];
            ActorDamage = (long)jsonData["actorDamage"];
            ActorCondiDPS = (long)jsonData["actorCondiDps"];
            ActorCondiDamage = (long)jsonData["actorCondiDamage"];
            ActorPowerDPS = (long)jsonData["actorPowerDps"];
            ActorPowerDamage = (long)jsonData["actorPowerDamage"];
        }

        public long ID { get; set; }

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
    }
}
