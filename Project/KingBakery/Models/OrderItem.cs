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
        [ForeignKey("Orders")]
        public int BillID { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }

        public virtual BakeryOption BakeryOption { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
