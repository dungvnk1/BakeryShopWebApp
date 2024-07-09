using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class OrderItem
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("BakeryOption")]
        public int BakeryID { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerID { get; set; }

        [ForeignKey("Orders")]
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual BakeryOption? BakeryOption { get; set; }
        public virtual Orders? Orders { get; set; }
    }
}
