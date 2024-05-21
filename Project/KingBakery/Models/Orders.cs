using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        [ForeignKey("Employee")]
        public int StaffID { get; set; }
        [ForeignKey("Employee")]
        public int ShipperID { get; set; }
        [ForeignKey("Vouchers")]
        public int VoucherID { get; set; }
        public DateTime DateTime { get; set; }
        public string AdrDelivery { get; set; }
        public float TotalPrice { get; set; }
        public string Status { get; set; }

        public virtual Customer customer { get; set; }
        public virtual Employee staff { get; set; }
        public virtual Employee shipper { get; set; }
        public virtual Vouchers vouchers { get; set; }
    }
}
