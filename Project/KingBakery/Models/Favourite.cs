using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Favourite
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("BakeryOption")]
        public int BakeryID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual BakeryOption BakeryOption { get; set; }
    }
}
