using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Models
{
    [Table("ParsedLogFile")]
    public class ParsedLogFile : BaseEntity
    {
        public string BossName { get; set; }
        
        public string Recorder { get; set; }

        public string Json { get; set; }

        public string Html { get; set; }

        public override string ToString()
        {
            return $"Boss: {BossName}\nRecorder: {Recorder}";
        }
    }
}
