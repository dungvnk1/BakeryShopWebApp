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

namespace KingBakery.Controllers
{
    public class OrdersController : Controller
    {
        private readonly KingBakeryContext _context;

        public OrdersController(KingBakeryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            var sum = orders.Where(o => o.Status == "Đã giao hàng").Sum(o => o.TotalPrice);
            var count = orders.Count();
            var today = DateTime.Now.ToShortDateString();

            var rtd = orders.Where(o => {
                var day = o.DateTime.Value.ToShortDateString();
                return (day == today) && (o.Status == "Đã giao hàng");
            })
                            .Sum(o => o.TotalPrice);

            ViewBag.Revenue = sum;
            ViewBag.NumberOrders = count - 1;
            ViewBag.RToday = rtd;
            return View(orders);
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
            return RedirectToAction("Index");
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
