﻿using System;
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
    [Authorize(Roles = "1")]
    public class BakeryOptionsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BakeryOptionsController(KingBakeryContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: BakeryOptions
        public async Task<IActionResult> Index()
        {
            var kingBakeryContext = _context.BakeryOption.Include(b => b.Bakery);
            return View(await kingBakeryContext.ToListAsync());
        }

        [AllowAnonymous]
        // GET: BakeryOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryOption = await _context.BakeryOption
                .Include(b => b.Bakery)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (bakeryOption == null)
            {
                return NotFound();
            }

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID != null)
            {
                var fav = _context.Favourite.Where(f => f.BakeryID == id && f.CustomerID == int.Parse(userID)).FirstOrDefault();
                ViewBag.fav = fav;
            }
            ViewBag.userID = userID;

            var kk = _context.Feedback.Where(f => f.BakeryID == id)
                                      .Include(f => f.Customer)
                                      .Include(f => f.Customer.Users)
                                      .ToList();
            ViewData["Feedbacks"] = kk;
            ViewData["BakeryOptions"] = _context.BakeryOption.Include(b => b.Bakery).Where(bo => bo.BakeryID == bakeryOption.BakeryID).ToList();
            return View(bakeryOption);
        }

        // GET: BakeryOptions/Create
        public IActionResult Create()
        {
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "Name");
            return View();
        }

        // POST: BakeryOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Size,Quantity,Price,Rating,Discount,BakeryID")] BakeryOption bakeryOption)
        {
            if (ModelState.IsValid)
            {
                if (_context.BakeryOption.Any(bo => bo.Size == bakeryOption.Size) && _context.BakeryOption.Any(bo => bo.BakeryID == bakeryOption.BakeryID))
                {
                    ViewBag.error = "Đã tồn tại size bánh";
                }
                else
                {
                    _context.Add(bakeryOption);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "Name", bakeryOption.BakeryID);
            return View(bakeryOption);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryOption = await _context.BakeryOption.FindAsync(id);
            if (bakeryOption == null)
            {
                return NotFound();
            }
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "Name", bakeryOption.BakeryID);
            return View(bakeryOption);
        }

        // POST: BakeryOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Size,Quantity,Price,Rating,Discount,BakeryID")] BakeryOption bakeryOption)
        {
            if (id != bakeryOption.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_context.BakeryOption.Any(bo => bo.Size == bakeryOption.Size) && _context.BakeryOption.Any(bo => bo.BakeryID == bakeryOption.BakeryID))
                {
                    ViewBag.error = "Đã tồn tại size bánh";
                    ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "Name", bakeryOption.BakeryID);
                    return View(bakeryOption);
                }
                else
                {
                    try
                    {
                        _context.Update(bakeryOption);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BakeryOptionExists(bakeryOption.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "Name", bakeryOption.BakeryID);
            return View(bakeryOption);
        }

        // GET: BakeryOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryOption = await _context.BakeryOption
                .Include(b => b.Bakery)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bakeryOption == null)
            {
                return NotFound();
            }

            return View(bakeryOption);
        }

        // POST: BakeryOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bakeryOption = await _context.BakeryOption.FindAsync(id);
            if (bakeryOption != null)
            {
                _context.BakeryOption.Remove(bakeryOption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BakeryOptionExists(int id)
        {
            return _context.BakeryOption.Any(e => e.ID == id);
        }
    }
}
