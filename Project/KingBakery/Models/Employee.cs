using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Employee
    {
        [Key]
        [ForeignKey("Users")]
        public int UserID { get; set; }
        public float Salary { get; set; }
        public DateTime HiredDate { get; set; }
        public string Status { get; set; }
        public virtual Users? User { get; set; }
    }
}
