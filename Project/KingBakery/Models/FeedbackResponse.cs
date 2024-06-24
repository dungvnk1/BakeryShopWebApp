namespace KingBakery.Models
{
    public class FeedbackResponse
    {
        public int ID { get; set; }
        public int FeedbackID { get; set; }
        public int StaffID { get; set; }
        public string? ReplyContent { get; set; }
        public DateTime ReplyDate { get; set; } = DateTime.Now;
        public virtual Feedback? Feedback { get; set; }
        public virtual Employee? Staff { get; set; }
    }
}
