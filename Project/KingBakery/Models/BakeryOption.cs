using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class BakeryOption
    {
        [Key]
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Size must be a positive number")]
        public int Size { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }
        public double? Rating { get; set; }
        public int? Discount { get; set; }
        [ForeignKey("Bakery")]
        public int BakeryID { get; set; }

        public virtual Bakery? Bakery { get; set; }
    }
}
