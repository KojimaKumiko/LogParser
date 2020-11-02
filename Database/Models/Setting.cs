using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    [Table("Settings")]
    public class Setting
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
