using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Vouchers
    {
        [Key]
        public int VoucherID { get; set; }
        public string? Code { get; set; }
        public int? VPercent { get; set; }
    }
}
