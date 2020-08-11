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

        public int SubGroup { get; set; }
        
        public bool HasCommander { get; set; }
        
        public int Weapons { get; set; }

        public string Name { get; set; }

        public int Condition { get; set; }

        public int Concentration { get; set; }

        public int Healing { get; set; }

        public int Instance { get; set; }

        public int Toughness { get; set; }

        public ParsedLogFile ParsedLogFile { get; set; }
    }
}
