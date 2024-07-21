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

namespace KingBakery.Controllers
{
    [Authorize(Roles = "1")]
    public class VouchersController : Controller
    {
        private readonly KingBakeryContext _context;

        public VouchersController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: Vouchers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vouchers.Include(v => v.Users).ToListAsync());
        }

        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vouchers = await _context.Vouchers.Include(v => v.Users)
                .FirstOrDefaultAsync(m => m.VoucherID == id);
            if (vouchers == null)
            {
                return NotFound();
            }

            return View(vouchers);
        }

        // GET: Vouchers/Create
        public IActionResult Create()
        {
            var users = _context.Users.Where(u => u.Role == 2).ToList();
            users.Insert(0, new Users { ID = 0, FullName = "All" }); // Add an "All" option
            ViewData["UserID"] = new SelectList(users, "ID", "FullName");
            var model = new Vouchers
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };
            return View(model);
        }

        // POST: Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vouchers vouchers)
        {
            if (vouchers.EndDate < vouchers.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be equal or greater than Start Date.");
            }
            if (ModelState.IsValid)
            {
                vouchers.Code = await GenerateUniqueVoucherCodeAsync();
                if (vouchers.UserID == 0)
                {
                    vouchers.UserID = null;
                }
                _context.Add(vouchers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var users = _context.Users.Where(u => u.Role == 2).ToList();
            users.Insert(0, new Users { ID = 0, FullName = "All" }); // Add an "All" option
            ViewData["UserID"] = new SelectList(users, "ID", "FullName");
            return View(vouchers);
        }

        private async Task<string> GenerateUniqueVoucherCodeAsync()
        {
            string code;
            do
            {
                code = GenerateRandomCode();
            } while (await _context.Vouchers.AnyAsync(v => v.Code == code));

            return code;
        }

        private string GenerateRandomCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // GET: Vouchers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vouchers = await _context.Vouchers.FindAsync(id);
            if (vouchers == null)
            {
                return NotFound();
            }
            var users = _context.Users.Where(u => u.Role == 2).ToList();
            users.Insert(0, new Users { ID = 0, FullName = "All" }); // Add an "All" option
            ViewData["UserID"] = new SelectList(users, "ID", "FullName");
            return View(vouchers);
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vouchers vouchers)
        {
            if (id != vouchers.VoucherID)
            {
                return NotFound();
            }
            if (vouchers.EndDate < vouchers.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be equal or greater than Start Date.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vouchers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VouchersExists(vouchers.VoucherID))
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
            var users = _context.Users.Where(u => u.Role == 2).ToList();
            users.Insert(0, new Users { ID = 0, FullName = "All" }); // Add an "All" option
            ViewData["UserID"] = new SelectList(users, "ID", "FullName");
            return View(vouchers);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vouchers = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.VoucherID == id);
            if (vouchers == null)
            {
                return NotFound();
            }

            return View(vouchers);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vouchers = await _context.Vouchers.FindAsync(id);
            if (vouchers != null)
            {
                _context.Vouchers.Remove(vouchers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VouchersExists(int id)
        {
            return _context.Vouchers.Any(e => e.VoucherID == id);
        }
    }
}
