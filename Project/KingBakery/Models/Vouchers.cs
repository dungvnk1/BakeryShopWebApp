using KingBakery.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Vouchers
    {
        [Key]
        public int VoucherID { get; set; }
        public required string Code { get; set; }
        [Range(1, 100, ErrorMessage = "Percent must be between 1 and 100")]
        public int VPercent { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [ForeignKey("Users")]
        public int? UserID { get; set; }

        public virtual Users? Users { get; set; }
    }
}