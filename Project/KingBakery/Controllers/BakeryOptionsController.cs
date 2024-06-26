﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using KingBakery.ViewModel;

namespace KingBakery.Controllers
{
    public class BakeryOptionsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BakeryOptionsController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: BakeryOptions
        public async Task<IActionResult> Index()
        {
            var kingBakeryContext = _context.BakeryOption.Include(b => b.Bakery);
            return View(await kingBakeryContext.ToListAsync());
        }

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
            var bakey = await _context.Bakery.Include(f => f.BakeryOptions.OrderByDescending(f => f.ID)).FirstOrDefaultAsync(m => m.ID == bakeryOption.BakeryID);

            if (bakeryOption == null)
            {
                return NotFound();
            }
            List<Feedback> feedback = await _context.Feedback.Include(f => f.FeedbackResponses).ThenInclude(f => f.Staff).ThenInclude(f => f.Users).Include(f => f.Customer).ThenInclude(f => f.Users).Where(b => b.BakeryID == id).ToListAsync();
            var model = new ProductDetailsViewModel
            {
                Bakerys = bakey,
                BakeryOptions = bakeryOption,
                Feedbacks = feedback
            };
            ViewData["BakeryOptions"] = _context.BakeryOption.Include(b => b.Bakery).Where(bo => bo.BakeryID == bakeryOption.BakeryID).ToList();
            return View(model);
        }


        // GET: BakeryOptions/Create
        public IActionResult Create()
        {
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "ID");
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
                _context.Add(bakeryOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "ID", bakeryOption.BakeryID);
            return View(bakeryOption);
        }

        // GET: BakeryOptions/Edit/5
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
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "ID", bakeryOption.BakeryID);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["BakeryID"] = new SelectList(_context.Bakery, "ID", "ID", bakeryOption.BakeryID);
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
