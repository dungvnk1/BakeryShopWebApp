using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class BakeryDetail
    {
        [Key]
        [ForeignKey("Bakery")]
        public int ID { get; set; }
        public int? Size { get; set; }
        public int? Quantity { get; set; }
        public float? Price { get; set; }
        public float? Rating { get; set; }
        public int? Discount { get; set; }
        public virtual Bakery? BakeryID { get; set; }
    }
}
