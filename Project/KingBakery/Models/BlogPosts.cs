using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class BlogPosts
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
    }
}
