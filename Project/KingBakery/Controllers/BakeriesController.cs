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
using System.Drawing.Printing;
using X.PagedList;
using KingBakery.Extensions;

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
        public async Task<IActionResult> Manage(int? page)
        {
            var bakeries = _context.Bakery
            .Include(b => b.BakeryOptions)
            .Include(b => b.Category)
            .ToList();
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<Bakery> lst = new PagedList<Bakery>(bakeries, pageNumber, pageSize);

            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(lst);       
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
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().RemoveDiacritics();
            }
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
        public async Task<IActionResult> Create(Bakery bakery, IFormFile uploadhinh)
        {
            if (uploadhinh == null)
            {
                ViewBag.error = "Vui lòng chọn file";
                ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
            }

            if (uploadhinh.Length == 0)
            {
                ViewBag.error = "File không có nội dung";
                ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
            }

            // Lưu dữ liệu vào cơ sở dữ liệu
            _context.Add(bakery);
            await _context.SaveChangesAsync();

            if (uploadhinh != null && uploadhinh.Length > 0)
            {
                int id = bakery.ID; // Sau khi lưu, bakery.ID sẽ có giá trị ID mới
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BakeryImg", uploadhinh.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadhinh.CopyToAsync(stream); // Sử dụng phiên bản async của CopyTo
                }

                Bakery bake = _context.Bakery.FirstOrDefault(x => x.ID == id);
                bake.Image = Path.Combine("/BakeryImg/", uploadhinh.FileName);
                await _context.SaveChangesAsync();
            }

            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id,  Bakery bakery, IFormFile uploadhinh)
        {
            if (id != bakery.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadhinh == null)
                    {
                        ViewBag.error = "Vui lòng chọn file";
                        ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                        return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
                    }

                    if (uploadhinh.Length == 0)
                    {
                        ViewBag.error = "File không có nội dung";
                        ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                        return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
                    }
                    _context.Update(bakery);
                    await _context.SaveChangesAsync();
                    if (uploadhinh != null && uploadhinh.Length > 0)
                    {
                        int idB = bakery.ID; // Sau khi lưu, bakery.ID sẽ có giá trị ID mới
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BakeryImg", uploadhinh.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadhinh.CopyToAsync(stream); // Sử dụng phiên bản async của CopyTo
                        }

                        Bakery bake = _context.Bakery.FirstOrDefault(x => x.ID == idB);
                        bake.Image = Path.Combine("/BakeryImg/", uploadhinh.FileName);
                        await _context.SaveChangesAsync();
                    }
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
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", bakery.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

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
