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
    public class BakeryDetailsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BakeryDetailsController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: BakeryDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.BakeryDetail.ToListAsync());
        }

        // GET: BakeryDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryDetail = await _context.BakeryDetail
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bakeryDetail == null)
            {
                return NotFound();
            }

            return View(bakeryDetail);
        }

        // GET: BakeryDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BakeryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Size,Quantity,Price,Rating,Discount")] BakeryDetail bakeryDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bakeryDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bakeryDetail);
        }

        // GET: BakeryDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryDetail = await _context.BakeryDetail.FindAsync(id);
            if (bakeryDetail == null)
            {
                return NotFound();
            }
            return View(bakeryDetail);
        }

        // POST: BakeryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Size,Quantity,Price,Rating,Discount")] BakeryDetail bakeryDetail)
        {
            if (id != bakeryDetail.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bakeryDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BakeryDetailExists(bakeryDetail.ID))
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
            return View(bakeryDetail);
        }

        // GET: BakeryDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakeryDetail = await _context.BakeryDetail
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bakeryDetail == null)
            {
                return NotFound();
            }

            return View(bakeryDetail);
        }

        // POST: BakeryDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bakeryDetail = await _context.BakeryDetail.FindAsync(id);
            if (bakeryDetail != null)
            {
                _context.BakeryDetail.Remove(bakeryDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BakeryDetailExists(int id)
        {
            return _context.BakeryDetail.Any(e => e.ID == id);
        }
    }
}
