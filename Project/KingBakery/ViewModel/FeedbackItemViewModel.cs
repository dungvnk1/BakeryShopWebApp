using KingBakery.Models;

namespace KingBakery.ViewModel
{
    public class FeedbackItemViewModel
    {
        public int OrderID { get; set; }
        public virtual List<OrderItem>? Items { get; set; }
        public virtual List<Feedback>? Feedbacks { get; set; }

        public virtual List<FeedbackResponse>? FeedbackResponses { get; set; }


    }
}
