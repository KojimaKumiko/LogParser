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

        public ParsedLogFile ParsedLogFile { get; set; }

        public List<DpsTarget> DpsTargets { get; set; }
    }
}
