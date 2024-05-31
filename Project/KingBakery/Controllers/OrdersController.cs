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

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var kingBakeryContext = _context.Orders.Include(o => o.Shipper).Include(o => o.Staff).Include(o => o.Vouchers);
            return View(await kingBakeryContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
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
