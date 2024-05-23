using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Customer")]
        public int? CustomerID { get; set; }
        [ForeignKey("Staff")]
        public int? StaffID { get; set; }
        [ForeignKey("Shipper")]
        public int? ShipperID { get; set; }
        [ForeignKey("Vouchers")]
        public int? VoucherID { get; set; }
        public DateTime? DateTime { get; set; }
        public string? AdrDelivery { get; set; }
        public double? TotalPrice { get; set; }
        public string? Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Staff { get; set; }
        public virtual Employee Shipper { get; set; }
        public virtual Vouchers Vouchers { get; set; }
    }
}
