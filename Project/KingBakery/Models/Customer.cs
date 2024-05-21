using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Customer
    {
        [Key]
        [ForeignKey("Users")]
        public int UserID { get; set; }
        public string Ranking { get; set; }
        public virtual Users? user { get; set; }
    }
}
