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

        [ForeignKey("BakeryOption")]
        public int CategoryID { get; set; }
        
        public virtual BakeryOption BakeryOption { get; set; }
        
    }
}
