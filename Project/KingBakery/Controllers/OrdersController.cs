using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace KingBakery.Controllers
{
    [Authorize(Roles = "1, 3")]
    public class OrdersController : Controller
    {
        private readonly KingBakeryContext _context;

        public OrdersController(KingBakeryContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, DateTime? fromDate, DateTime? toDate)
        {
            var orders = _context.Orders.ToList();

            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.DateTime >= fromDate.Value).ToList();
                ViewBag.From = fromDate.Value.ToString("yyyy-MM-dd");
            }

            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.DateTime <= toDate.Value.AddDays(1)).ToList();
                ViewBag.To = toDate.Value.ToString("yyyy-MM-dd");
            }

            var sum = orders.Where(o => o.Status == "Đã giao hàng").Sum(o => o.TotalPrice);
            var count = orders.Where(o => o.Status == "Đã giao hàng").ToList().Count();
            var today = DateTime.Now.ToShortDateString();

            var temp = _context.Orders.ToList();
            var rtd = temp.Where(o => {
                var day = o.DateTime.Value.ToShortDateString();
                return (day == today) && (o.Status == "Đã giao hàng");
            })
                            .Sum(o => o.TotalPrice);

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var od = orders.OrderByDescending(o => o.DateTime).ToPagedList(pageNumber, pageSize);

            ViewBag.Revenue = sum;
            ViewBag.NumberOrders = count;
            ViewBag.RToday = rtd;
            return View(od);
        }

        public IActionResult Details(int id)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);

            var uid = items.FirstOrDefault().CustomerID;

            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Phone = order.PhoneNumber;
            ViewBag.Address = order.AdrDelivery;
            ViewBag.Status = order.Status;
            ViewBag.Note = order.Note;
            ViewBag.Payment = order.Payment;

            var voucherPercent = 0;
            if (order.VoucherID != null)
            {
                voucherPercent = _context.Vouchers.FirstOrDefault(v => v.VoucherID == order.VoucherID).VPercent;
            }
            ViewBag.VoucherPercent = voucherPercent;

            var staff = _context.Users.FirstOrDefault(u => u.ID == order.StaffID);
            var shipper = _context.Users.FirstOrDefault(u => u.ID == order.ShipperID);
            var voucher = _context.Vouchers.FirstOrDefault(v => v.VoucherID == order.VoucherID);
            string staffName = "Chưa có", shipperName = "Chưa có", code = "Không có";
            if (staff != null)
            {
                staffName = staff.FullName;
            }
            if (shipper != null)
            {
                shipperName = shipper.FullName;
            }
            if (voucher != null && voucher.Code != null)
            {
                code = voucher.Code;
            }

            ViewBag.Staff = staffName;
            ViewBag.Shipper = shipperName;
            ViewBag.Voucher = code;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(items);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int parsedUserId))
            {
                var user = _context.Users.FirstOrDefault(u => u.ID == parsedUserId);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            else
            {

                return BadRequest("Invalid user ID.");
            }
        }

        public IActionResult Cancel(int id, string reason)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == id).ToList();

            var order = _context.Orders.Find(id);
            foreach (var item in items)
            {
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                bakery.Quantity += item.Quantity;
                _context.BakeryOption.Update(bakery);
                _context.SaveChanges();
            }

            if (reason == "default") reason = "Xin lỗi quý khách, hiện tại shop không thể ship hàng. Mong quý khách thông cảm.";

            order.Status = "Bị từ chối";
            order.DenyReason = reason;
            _context.Orders.Update(order);
            _context.SaveChanges();

            var cusId = _context.OrderItem.FirstOrDefault(o => o.OrderID == id).CustomerID;
            var email = _context.Users.FirstOrDefault(u => u.ID == cusId).Email;
            SendEmailCancel(email, reason);

            return RedirectToAction("Index");
        }

        public JsonResult GetCancelReason(int id)
        {
            var mess = _context.Orders.FirstOrDefault(o => o.ID == id);
            var reason = "Xin lỗi quý khách, hiện tại shop không thể ship hàng. Mong quý khách thông cảm.";
            if (mess != null)
            {
                if (mess.DenyReason != null)
                {
                    reason = mess.DenyReason;
                }
            }
            return Json(new
            {
                reason
            });
        }

        public void SendEmailCancel(string email, string reason)
        {
            string fromMail = "hung080104@gmail.com";
            string fromPassword = "popa aogx skig wdpe";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "[King Bakery] Đơn hàng bị từ chối";
            message.To.Add(new MailAddress(email));
            message.Body = $"<html><body> {reason}" +
                           $"<br>Nếu quý khách đã thanh toán đơn hàng, cửa hàng sẽ sớm liên hệ và hoàn tiền." +
                           $"<br><br>Liên hệ:" +
                           $"<br>SĐT: 0975861471" +
                           $"<br>Facebook: King Bakery Shop" +
                           $"<br><br>King Bakery chân thành xin lỗi quý khách vì sự bất tiện này. </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CustomerID,StaffID,ShipperID,VoucherID,DateTime,AdrDelivery,TotalPrice,Status")] Orders orders)
        {
            if (ModelState.IsValid)
            {


                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("Create"); // Chuyển hướng về trang chủ
            }
            return View();
        }



        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["ShipperID"] = new SelectList(_context.Employee, "UserID", "UserID", orders.ShipperID);
            ViewData["StaffID"] = new SelectList(_context.Employee, "UserID", "UserID", orders.StaffID);
            ViewData["VoucherID"] = new SelectList(_context.Vouchers, "VoucherID", "VoucherID", orders.VoucherID);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CustomerID,StaffID,ShipperID,VoucherID,DateTime,AdrDelivery,TotalPrice,Status")] Orders orders)
        {
            if (id != orders.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShipperID"] = new SelectList(_context.Employee, "UserID", "UserID", orders.ShipperID);
            ViewData["StaffID"] = new SelectList(_context.Employee, "UserID", "UserID", orders.StaffID);
            ViewData["VoucherID"] = new SelectList(_context.Vouchers, "VoucherID", "VoucherID", orders.VoucherID);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.Shipper)
                .Include(o => o.Staff)
                .Include(o => o.Vouchers)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}
