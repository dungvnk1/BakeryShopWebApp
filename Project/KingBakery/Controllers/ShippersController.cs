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
using Microsoft.AspNetCore.Authorization;

namespace KingBakery.Controllers
{
    public class ShippersController : Controller
    {
        private readonly KingBakeryContext _context;

        public ShippersController(KingBakeryContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "4")]
        // GET: Shippers
        public async Task<IActionResult> Order()
        {
            int shipperId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var orders = await _context.Orders.Where(o => o.ShipperID == shipperId).ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Accept(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false });
            }

            order.Status = "Đang giao hàng"; // Cập nhật trạng thái từ "Đã chấp nhận" sang "Đang giao hàng"
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false });
            }

            order.Status = "Đã từ chối";
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string status)
        {
            var order = _context.Orders.Find(orderId); // Assuming Orders is the DbSet for orders
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false });
            }

            order.Status = "Đã giao hàng"; // Cập nhật trạng thái từ "Đang giao hàng" sang "Đã giao hàng"
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



        //
        // // GET: Shippers/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var shipper = await _context.Shipper
        //         .Include(s => s.Users)
        //         .FirstOrDefaultAsync(m => m.UserID == id);
        //     if (shipper == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(shipper);
        // }
        //
        // // GET: Shippers/Create
        // public IActionResult Create()
        // {
        //     ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
        //     return View();
        // }
        //
        // // POST: Shippers/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("UserID,Salary,HiredDate,Status")] Shipper shipper)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(shipper);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", shipper.UserID);
        //     return View(shipper);
        // }
        //
        // // GET: Shippers/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var shipper = await _context.Shipper.FindAsync(id);
        //     if (shipper == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", shipper.UserID);
        //     return View(shipper);
        // }
        //
        // // POST: Shippers/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("UserID,Salary,HiredDate,Status")] Shipper shipper)
        // {
        //     if (id != shipper.UserID)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(shipper);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!ShipperExists(shipper.UserID))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", shipper.UserID);
        //     return View(shipper);
        // }
        //
        // // GET: Shippers/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var shipper = await _context.Shipper
        //         .Include(s => s.Users)
        //         .FirstOrDefaultAsync(m => m.UserID == id);
        //     if (shipper == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(shipper);
        // }
        //
        // // POST: Shippers/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var shipper = await _context.Shipper.FindAsync(id);
        //     if (shipper != null)
        //     {
        //         _context.Shipper.Remove(shipper);
        //     }
        //
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
        //
        // private bool ShipperExists(int id)
        // {
        //     return _context.Shipper.Any(e => e.UserID == id);
        // }
        // // List orders assigned to the logged-in shipper
        // public async Task<IActionResult> MyOrders()
        // {
        //     var userRole = User.FindFirstValue(ClaimTypes.Role);
        //     if (userRole != "4") // Checks if the user role is not "4"
        //     {
        //         return Unauthorized(); // Or any other appropriate response
        //     }
        //
        //     string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (userIdValue == null)
        //     {
        //         return Unauthorized();
        //     }
        //
        //     if (!int.TryParse(userIdValue, out int userId))
        //     {
        //         return BadRequest("Invalid user ID");
        //     }
        //
        //     List<Orders>? orders = await _context.Orders
        //                                    .Where(o => o.ShipperID == userId) // Filter orders by shipper ID
        //                                    .ToListAsync();
        //     return View(orders);
        // }
    }
}
