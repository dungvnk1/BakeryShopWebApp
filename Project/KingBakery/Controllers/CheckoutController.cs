using KingBakery.Data;
using KingBakery.Models;
using KingBakery.Services;
using KingBakery.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly KingBakeryContext _context;
        private readonly IVnPayService _vnPayService;

        public CheckoutController(KingBakeryContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
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
        public IActionResult CreateBill(string address, string number, string note, string voucher, string payment)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var user = _context.Users.FirstOrDefault(o => o.ID == uid);

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

            var oid = new Random().Next(100000, 999999);
            while (_context.Orders.FirstOrDefault(o => o.ID == oid) != null) { oid = new Random().Next(100000, 999999); }

            double total = 0;
            foreach (var item in Order)
            {
                var temp = _context.OrderItem.FirstOrDefault(o => o.ID == item.ID);
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                total += temp.Price;
            }
            total += 20000;
            double transTotal = total;
            if(vch != null)
            {
                transTotal = total - (total * vch.VPercent / 100);
            }

            if (payment == "Thanh toán VNPAY")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = transTotal,
                    CreatedDate = DateTime.Now,
                    Description = $"{address}_{number}",
                    FullName = user == null ? "" : user.FullName,
                    OrderId = oid,
                };
                HttpContext.Session.SetString("Address", address);
                HttpContext.Session.SetString("Number", number);
                HttpContext.Session.SetString("VoucherID", vid == null ? "" : vid.ToString());
                HttpContext.Session.SetString("Note", note ?? "");
                HttpContext.Session.SetString("OrderId", oid.ToString());
                var vnPayUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
                Console.WriteLine(vnPayUrl); // Log URL to check
                return Redirect(vnPayUrl);
            }
            var bill = new Orders()
            {
                DateTime = DateTime.Now,
                AdrDelivery = address,
                PhoneNumber = number,
                VoucherID = vid,
                Status = "Đã đặt hàng",
                Payment = "COD",
                Note = note
            };
            _context.Orders.Add(bill);
            _context.SaveChanges();

            foreach (var item in Order)
            {
                var temp = _context.OrderItem.FirstOrDefault(o => o.ID == item.ID);
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                bakery.Quantity -= item.Quantity;
                temp.OrderID = bill.ID;
                _context.OrderItem.Update(temp);
                _context.SaveChanges();
                _context.BakeryOption.Update(bakery);
                _context.SaveChanges();
            }
            bill.TotalPrice = total;
            if (vch != null && vch.Quantity > 0)
            {
                bill.TotalPrice -= (bill.TotalPrice * vch.VPercent / 100);
                vch.Quantity--;
                _context.Vouchers.Update(vch);
                _context.SaveChanges();
            }
            _context.Orders.Update(bill);
            _context.SaveChanges();

            HttpContext.Session.Clear();
            if (user != null)
            {
                SendEmailOrder(user.Email);
            }

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
                DateTime currentDate = DateTime.Now;
                if (voucher.Quantity > 0 && voucher.EndDate >= currentDate && voucher.StartDate <= currentDate)
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
                if ( (voucher.UserID != null && voucher.UserID != uid))
                {
                    inuse = true;
                }
            }

            return Json(new
            {
                exist,
                remain,
                inuse,
                percent
            });
        }

        public IActionResult PaymentFail() { return View(); }

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("Index");
            }

            //Update Database
            var address = HttpContext.Session.GetString("Address");
            var number = HttpContext.Session.GetString("Number");
            var voucher = HttpContext.Session.GetString("VoucherID");
            var note = HttpContext.Session.GetString("Note");
            var oid = HttpContext.Session.GetString("OrderId");
            int? vid = null;
            if (!string.IsNullOrEmpty(voucher))
            {
                vid = int.Parse(voucher);
            }
            var vch = _context.Vouchers.FirstOrDefault(v => v.VoucherID == vid);
            if (string.IsNullOrEmpty(note))
            {
                note = null;
            }


            var bill = new Orders()
            {
                DateTime = DateTime.Now,
                AdrDelivery = address,
                PhoneNumber = number,
                VoucherID = vid,
                Status = "Đã đặt hàng",
                Payment = "VNP",
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
            if (vch != null && vch.Quantity > 0)
            {
                bill.TotalPrice -= (bill.TotalPrice * vch.VPercent / 100);
                vch.Quantity--;
                _context.Vouchers.Update(vch);
                _context.SaveChanges();
            }
            _context.Orders.Update(bill);
            _context.SaveChanges();

            HttpContext.Session.Clear();

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var user = _context.Users.FirstOrDefault(o => o.ID == uid);

            if(user != null)
            {
                SendEmailOrder(user.Email);
            }
            TempData["Message"] = $"Thanh toán VN Pay thành công!";
            return RedirectToAction("Index");
        }

        public void SendEmailOrder(string email)
        {
            string fromMail = "hung080104@gmail.com";
            string fromPassword = "popa aogx skig wdpe";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "[King Bakery] Đặt hàng thành công";
            message.To.Add(new MailAddress(email));
            message.Body = $"<html><body> Quý khách đã đặt hàng thành công. Đơn hàng sẽ được vận chuyển sớm nhất có thể." +
                           $"<br> Thời gian: {DateTime.Now}" +
                           $"<br><br>King Bakery trân trọng cảm ơn quý khách! </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}
