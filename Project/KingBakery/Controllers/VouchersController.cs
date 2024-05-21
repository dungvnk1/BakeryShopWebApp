﻿using System;
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
            return View(await _context.Vouchers.ToListAsync());
        }

        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Vouchers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoucherID,Code,VPercent")] Vouchers vouchers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vouchers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vouchers);
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
            return View(vouchers);
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoucherID,Code,VPercent")] Vouchers vouchers)
        {
            if (id != vouchers.VoucherID)
            {
                return NotFound();
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
