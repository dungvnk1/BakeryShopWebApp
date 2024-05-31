using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace KingBakery.Controllers
{
    public class BakeriesController : Controller
    {
        private readonly KingBakeryContext _context;

        public BakeriesController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: Bakeries manager
        public async Task<IActionResult> Manage()
        {
            var kingBakeryContext = _context.Bakery.Include(b => b.Category);
            return View(await kingBakeryContext.ToListAsync());
        }

        // GET: Bakeries by CategoryID
        public async Task<IActionResult> Index(int id)
        {
            var bakery = _context.Bakery.Include(b => b.Category)
                                        .Include(b => b.BakeryOptions)
                                        .Where(b => b.CategoryID == id)
                                        .ToList();
            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(bakery);
        }

        //Search Bakery
        [HttpPost]
        public async Task<IActionResult> Index(string? keyword)
        {
            var bakery = _context.Bakery.Include(b => b.Category)
                                        .Include(b => b.BakeryOptions)
                                        .Where(b => b.Name.Contains(keyword))
                                        .ToList();
            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(bakery);
        }

        // GET: Bakeries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakery = await _context.Bakery
                .Include(b => b.Category)
                .Include(b => b.BakeryOptions)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (bakery == null)
            {
                return NotFound();
            }
            var bakeryOption = _context.BakeryOption.Include(b => b.Bakery).FirstOrDefault(b => b.BakeryID == id);
            if (bakeryOption == null)
            {
                return NotFound();
            }
            ViewData["Quantity"] = bakeryOption.Quantity;
            ViewData["Price"] = bakeryOption.Price;
            return View(bakery);
        }

        // GET: Bakeries/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        // POST: Bakeries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Image,Description,CategoryID")] Bakery bakery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bakery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID);
            return View(bakery);
        }

        // GET: Bakeries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakery = await _context.Bakery.FindAsync(id);
            if (bakery == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "ID", bakery.CategoryID);
            return View(bakery);
        }

        // POST: Bakeries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Image,Description,CategoryID")] Bakery bakery)
        {
            if (id != bakery.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bakery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BakeryExists(bakery.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "ID", bakery.CategoryID);
            return View(bakery);
        }

        // GET: Bakeries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bakery = await _context.Bakery
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bakery == null)
            {
                return NotFound();
            }

            return View(bakery);
        }

        // POST: Bakeries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bakery = await _context.Bakery.FindAsync(id);
            if (bakery != null)
            {
                _context.Bakery.Remove(bakery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        private bool BakeryExists(int id)
        {
            return _context.Bakery.Any(e => e.ID == id);
        }
    }
}
