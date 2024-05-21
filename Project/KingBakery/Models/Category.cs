using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Category
    {
        [Key]
        [ForeignKey("Bakery")]
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual Category? CategoryID { get; set; }
    }
}
