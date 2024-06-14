using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Vouchers
    {
        [Key]
        public int VoucherID { get; set; }
        public required string Code { get; set; }
        public int VPercent { get; set; }
        public int Quantity { get; set; }
    }
}
