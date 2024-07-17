using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Staff")]
        public int? StaffID { get; set; }
        //[ForeignKey("Shipper")]
        public int? ShipperID { get; set; }
        [ForeignKey("Vouchers")]
        public int? VoucherID { get; set; }
        public DateTime? DateTime { get; set; }
        public string? AdrDelivery { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public double? TotalPrice { get; set; }
        public string? Payment { get; set; }
        public string? Status { get; set; }
        public string? DenyReason { get; set; }
        public virtual Employee? Staff { get; set; }
        public virtual Users? Shipper { get; set; }
        public virtual Vouchers? Vouchers { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
