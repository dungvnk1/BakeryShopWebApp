using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class StaffsController : Controller
    {
        private readonly KingBakeryContext _context;

        public StaffsController(KingBakeryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders.Where(o => o.ShipperID == null && o.Status == "Đã đặt hàng").ToList();
            
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Users)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Salary,HiredDate,Status")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", staff.UserID);
            return View(staff);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", staff.UserID);
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,Salary,HiredDate,Status")] Staff staff)
        {
            if (id != staff.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.UserID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", staff.UserID);
            return View(staff);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Users)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.UserID == id);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = _context.Orders.Include(o => o.Shipper).ToList();
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

        [Authorize(Roles = "3")]
        public async Task<IActionResult> AssignShipper1()
        {
            var orders = _context.Orders.Where(o => o.ShipperID == null && o.Status == "Đã đặt hàng").ToList();
            var shippers = _context.Users.Where(o => o.Role == 4).ToList();
            ViewBag.Shippers = shippers;
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int orderId, int shipperId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ID == orderId && o.ShipperID == null);
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng hợp lệ để cập nhật.");
            }

            order.ShipperID = shipperId;
            order.StaffID = uid;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}