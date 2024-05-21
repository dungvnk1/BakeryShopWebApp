using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Bakery
    {
        [Key]
        public int BakeryId { get; set; }
        public string Name { get; set; }
        public string? Image {  get; set; }
        public string? Description { get; set; }
        public int CategoryID { get; set; }

    }
}
