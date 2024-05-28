using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Bakery
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        
        public virtual Category? Category { get; set; }

        public virtual ICollection<BakeryOption>? BakeryOptions { get; set; }

    }
}
