using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Feedback
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        [ForeignKey("BakeryDetail")]
        public int BakeryID { get; set; }
        public string ContentFB { get; set; }

        public virtual Customer customer { get; set; }
        public virtual BakeryDetail bakeryDetail { get; set; }
    }
}
