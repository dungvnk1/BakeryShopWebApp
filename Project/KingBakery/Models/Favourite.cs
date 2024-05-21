using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Favourite
    {
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("BakeryDetail")]
        public int BakeryID { get; set; }

        public virtual Customer customer { get; set; }
        public virtual BakeryDetail bakeryDetail { get; set; }
    }
}
