using KingBakery.Models;

namespace KingBakery.ViewModel
{
    public class ProductDetailsViewModel
    {
        public Bakery? Bakerys { get; set; }
        public BakeryOption? BakeryOptions { get; set; }
        public List<Feedback>? Feedbacks { get; set; }
        public List<Orders>? Orders { get; set;}
    }
}
