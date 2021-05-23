using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public long ID { get; set; }
    }
}
