using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly KingBakeryContext _context;

        public CheckoutController(KingBakeryContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Email = user.Email;
            ViewBag.Phone = user.PhoneNumber;
            ViewBag.Address = user.Address;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(Order);
        }

        public List<OrderItem> Order
        {
            get
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uid = 0;
                if (userID != null)
                {
                    uid = int.Parse(userID);
                }

                var data = _context.OrderItem.Include(o => o.BakeryOption)
                                             .Where(o => o.CustomerID == uid)
                                             .Where(o => o.OrderID == 0)
                                             .ToList();
                if (data == null)
                {
                    data = new List<OrderItem>();
                }
                return data;
            }
        }

        [HttpPost]
        public IActionResult CreateBill(string address, string number, string note, string voucher)
        {
            string tempstr = "" + address[0];
            for (int i = 1; i < address.Length; i++)
            {
                if (address[i] == ' ' && tempstr[tempstr.Length - 1] == ' ') { }
                else
                {
                    tempstr += address[i];
                }
            }
            address = tempstr.Trim();

            var vch = _context.Vouchers.FirstOrDefault(v => v.Code == voucher);
            int? vid = null;
            if (vch != null)
            {
                vid = vch.VoucherID;
            }

            var bill = new Orders()
            {
                DateTime = DateTime.Now,
                AdrDelivery = address,
                PhoneNumber = number,
                VoucherID = vid,
                Status = "Đã đặt hàng",
                Note = note
            };
            _context.Orders.Add(bill);
            _context.SaveChanges();

            double total = 0;
            foreach (var item in Order)
            {
                var temp = _context.OrderItem.FirstOrDefault(o => o.ID == item.ID);
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                bakery.Quantity -= item.Quantity;
                temp.OrderID = bill.ID;
                total += temp.Price;
                _context.OrderItem.Update(temp);
                _context.SaveChanges();
                _context.BakeryOption.Update(bakery);
                _context.SaveChanges();
            }
            bill.TotalPrice = total + 20000;
            if (vch != null)
            {
                bill.TotalPrice -= (bill.TotalPrice * vch.VPercent / 100);
            }
            _context.Orders.Update(bill);
            _context.SaveChanges();

            HttpContext.Session.Clear();

            return Redirect("/Home");
        }

        public JsonResult UseVoucher(string code)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var voucher = _context.Vouchers.FirstOrDefault(o => o.Code == code);

            Boolean inuse = false;
            Boolean exist = false;
            Boolean remain = false;
            var percent = 0;
            if (voucher != null)
            {
                exist = true;
                percent = voucher.VPercent;
                if (voucher.Quantity > 0)
                {
                    remain = true;
                }

                var odusevoucher = _context.Orders.Where(o => o.VoucherID == voucher.VoucherID).ToList();
                foreach (var o in odusevoucher)
                {
                    var oid = o.ID;
                    var check = _context.OrderItem.FirstOrDefault(i => i.OrderID == oid && i.CustomerID == uid);
                    if (check != null)
                    {
                        inuse = true;
                    }
                }
            }
            if(remain && !inuse)
            {
                voucher.Quantity--;
            }
            _context.Vouchers.Update(voucher);
            _context.SaveChanges();
            return Json(new
            {
                exist,
                remain,
                inuse,
                percent
            });
        }
    }
}
