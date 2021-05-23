using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class LogPlayerDto
    {
        public LogPlayerDto()
        {
        }

        public LogPlayerDto(JObject playerData)
        {
            if (playerData == null)
            {
                throw new ArgumentNullException(nameof(playerData));
            }

            AccountName = (string)playerData["account"];
            SubGroup = (long)playerData["group"];
            HasCommander = (bool)playerData["hasCommanderTag"];
            Profession = (string)playerData["profession"];
            Name = (string)playerData["name"];
            Condition = (long)playerData["condition"];
            Concentration = (long)playerData["concentration"];
            Healing = (long)playerData["healing"];
            Toughness = (long)playerData["toughness"];
            Instance = (long)playerData["instanceID"];
        }

        public LogPlayerDto(JObject playerData, List<DpsTargetDto> dpsTargetDtos) : this(playerData)
        {
            DpsTargets = dpsTargetDtos ?? throw new ArgumentNullException(nameof(dpsTargetDtos));
        }

        public long ID { get; set; }

        public string AccountName { get; set; }

        public long SubGroup { get; set; }

        public bool HasCommander { get; set; }

        public string Profession { get; set; }

        public string Name { get; set; }

        public long Condition { get; set; }

        public long Concentration { get; set; }

        public long Healing { get; set; }

        public long Toughness { get; set; }

        public long Instance { get; set; }

        public List<DpsTargetDto> DpsTargets { get; set; }
    }
}
