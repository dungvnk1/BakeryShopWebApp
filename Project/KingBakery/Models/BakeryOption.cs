using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class BakeryOption
    {
        [Key]
        public int ID { get; set; }
        public int? Size { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Rating { get; set; }
        public int? Discount { get; set; }
        [ForeignKey("Bakery")]
        public int BakeryID {  get; set; }

        public virtual Bakery Bakery { get; set; }
    }
}
