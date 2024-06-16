using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Shipper
    {
        [Key]
        [ForeignKey("Users")]
        public int UserID { get; set; }
        public double Salary { get; set; }
        public DateTime HiredDate { get; set; }
        public string Status { get; set; }
        public virtual Users? Users { get; set; }
    }
}