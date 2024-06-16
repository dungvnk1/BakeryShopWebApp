using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;

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
            var kingBakeryContext = _context.Staff.Include(s => s.Users);
            return View(await kingBakeryContext.ToListAsync());
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

        public async Task<IActionResult> AssignShipper()
        {
            // Đảm bảo rằng đơn hàng và shipper có sẵn
            var orders = await _context.Orders.Include(o => o.Shipper).Where(o => o.ShipperID == null).ToListAsync();
            var shippers = await _context.Shipper.ToListAsync();

            ViewData["ShipperID"] = new SelectList(shippers, "ID", "Name");

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignShipper(int[] orderIDs, int shipperID)
        {
            if (orderIDs == null || orderIDs.Length == 0)
            {
                return NotFound("No orders provided.");
            }

            // Directly check if the shipper exists in the database
            var shipperExists = await _context.Shipper.AnyAsync(s => s.UserID == shipperID);
            if (!shipperExists)
            {
                return NotFound("Shipper does not exist.");
            }

            var ordersToUpdate = await _context.Orders.Where(o => orderIDs.Contains(o.ID) && o.ShipperID == null).ToListAsync();

            if (ordersToUpdate.Count == 0)
            {
                return NotFound("No valid orders found to update.");
            }

            foreach (var order in ordersToUpdate)
            {
                order.ShipperID = shipperID;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}