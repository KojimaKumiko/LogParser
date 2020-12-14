using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Models
{
    [Table("LogFile")]
    public class LogFile : BaseEntity
    {
        public string BossName { get; set; }

        public string Recorder { get; set; }

        public ParsedLogFile ParsedLogFile { get; set; }
    }
}
