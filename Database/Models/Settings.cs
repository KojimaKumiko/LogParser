using Database.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    [Table("Settings")]
    public class Settings
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }

        public int DisplayOrder { get; set; }

        public SettingsType SettingsType { get; set; }
    }
}
