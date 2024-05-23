using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        public required string Name { get; set; }
    }
}
