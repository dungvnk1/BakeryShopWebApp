using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Helpers;

namespace KingBakery.Models
{
    public class Bakery
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Image { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public bool isDeleted { get; set; } = false;

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<BakeryOption>? BakeryOptions { get; set; }

       

    }
}
