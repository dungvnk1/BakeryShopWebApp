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
        [ForeignKey("BakeryOption")]
        public int BakeryID { get; set; }
        [ForeignKey("OrderItem")]
        public int OrderID { get; set; }
        public DateTime FeedbackDate { get; set; } = DateTime.Now;
        public string? ContentFB { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual BakeryOption? Bakery { get; set; }
        public virtual OrderItem? OrderItems { get; set; }
        public virtual ICollection<FeedbackResponse>? FeedbackResponses { get; set; }
    }
}
