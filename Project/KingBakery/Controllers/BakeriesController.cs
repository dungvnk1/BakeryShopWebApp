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
using System.Globalization;
using System.Text;
using System.Security.Claims;
using KingBakery.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.EntityFrameworkCore.Internal;

namespace KingBakery.Controllers
{
    public class BakeriesController : Controller
    {
        private readonly KingBakeryContext _context;

        public BakeriesController(KingBakeryContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "1")]
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
        public async Task<IActionResult> Index()
        {
            var bakery = _context.Bakery.Include(b => b.Category)
                                         .Include(b => b.BakeryOptions)
                                          .ToList();
            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(bakery);
        }

        //Search Bakery
        [HttpPost]
        public async Task<IActionResult> Index(string? keyword, int? categoryID, string? priceRange, int? page)
        {
            // Bắt đầu với truy vấn cơ bản
            var bakeries = from b in _context.Bakery select b;

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                string tempstr = "" + keyword[0];
                for (int i = 1; i < keyword.Length; i++)
                {
                    if (keyword[i] == ' ' && tempstr[tempstr.Length - 1] == ' ') { }
                    else
                    {
                        tempstr += keyword[i];
                    }
                }
                keyword = tempstr.Trim();
                if (keyword.Length > 100)
                {
                    ModelState.AddModelError("Keyword", "Từ khóa tìm kiếm không được vượt quá 100 ký tự.");
                    keyword = null;
                }
                keyword = RemoveDiacritics(keyword);
            }

            var bakeryQuery = _context.Bakery.Include(b => b.Category)
                                             .Include(b => b.BakeryOptions);

            var bakeryList = await bakeryQuery.ToListAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                bakeryList = bakeryList.Where(b => RemoveDiacritics(b.Name).Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (categoryID.HasValue)
            {
                bakeryList = bakeryList.Where(b => b.CategoryID == categoryID.Value).ToList();
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceParts = priceRange.Split('-');
                if (priceParts.Length == 2)
                {
                    if (Decimal.TryParse(priceParts[0], out decimal minPrice) && Decimal.TryParse(priceParts[1], out decimal maxPrice))
                    {
                        bakeryList = bakeryList.Where(b => b.BakeryOptions.Any(bo => (decimal)bo.Price >= minPrice && (decimal)bo.Price <= maxPrice)).ToList();
                    }
                }
                else if (priceRange.EndsWith("+") && Decimal.TryParse(priceRange.TrimEnd('+'), out decimal minPriceOnly))
                {
                    bakeryList = bakeryList.Where(b => b.BakeryOptions.Any(bo => (decimal)bo.Price >= minPriceOnly)).ToList();
                }
            }

            var categories = await _context.Category.ToListAsync();
            ViewData["Categories"] = categories;

            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<Bakery> lst = new PagedList<Bakery>(bakeryList, pageNumber, pageSize);
            ViewData["keyword"] = keyword;
            ViewData["categoryID"] = categoryID;
            ViewData["priceRange"] = priceRange;

            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_BakeryListPartial", lst);
            }



            return View(lst);
        }

        [Authorize(Roles = "1")]
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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
            var kk = _context.Feedback.Include(f => f.Customer)
                                      .Include(f => f.Customer.Users)
                                      .Where(f => f.BakeryID == bakeryOption.ID)
                                      .ToList();
            ViewData["Feedbacks"] = kk;

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID != null)
            {
                var fav = _context.Favourite.Where(f => f.BakeryID == bakeryOption.ID && f.CustomerID == int.Parse(userID)).FirstOrDefault();
                ViewBag.fav = fav;
            }
            ViewBag.userID = userID;

            ViewData["Quantity"] = bakeryOption.Quantity;
            ViewData["Price"] = bakeryOption.Price;
            return View(bakery);
        }

        [Authorize(Roles = "1")]
        // GET: Bakeries/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        [Authorize(Roles = "1")]
        // POST: Bakeries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBackeryViewModel bakeryView, IFormFile uploadhinh)
        {
            var bakery = bakeryView.Backery;
            var bakeryOption = bakeryView.BackeryOption;
            if (_context.Bakery.Any(b => b.Name == bakery.Name) && _context.BakeryOption.Any(bo => bo.Size == bakeryOption.Size))
            {
                ViewBag.Errorness = "bánh đã tồn tại";
                ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                return View(bakeryView); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
            }
                if (uploadhinh == null)
            {
                ViewBag.error = "Vui lòng chọn file";
                ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                return View(bakeryView); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
            }

            if (uploadhinh.Length == 0)
            {
                ViewBag.error = "File không có nội dung";
                ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                return View(bakeryView); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
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
            bakeryOption.BakeryID = bakery.ID;
            _context.Add(bakeryOption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));

            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "1")]
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

        [Authorize(Roles = "1")]
        // POST: Bakeries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bakery bakery, IFormFile uploadhinh)
        {
            if (id != bakery.ID)
            {
                return NotFound();
            }

           

                try
                {
                    if (_context.Bakery.Any(b => b.Name == bakery.Name) )
                    {
                        ViewBag.Errorness = "bánh đã tồn tại";
                        ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                        return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
                    }
                    if (uploadhinh == null)
                    {
                        ViewBag.errorrr = "Vui lòng chọn file";
                        ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", bakery.CategoryID); // Thiết lập lại SelectList
                        return View(bakery); // Trả lại view cùng với đối tượng bakery để duy trì dữ liệu nhập
                    }

                    if (uploadhinh.Length == 0)
                    {
                        ViewBag.errorrr = "File không có nội dung";
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
            
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "ID", bakery.CategoryID);
            return View(bakery);
        }

        [Authorize(Roles = "1")]
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

        [Authorize(Roles = "1")]
        // POST: Bakeries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bakery = await _context.Bakery.FindAsync(id);        
            if (bakery != null)
            {
                bakery.isDeleted = true;
                _context.Update(bakery);
                var bakeryOptions = _context.BakeryOption.Where(bo => bo.BakeryID == id).ToList();
                foreach (var option in bakeryOptions)
                {      
                    _context.Remove(option);
                }
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