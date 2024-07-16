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
    public class BlogPostsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BlogPostsController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPosts = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPosts == null)
            {
                return NotFound();
            }

            return View(blogPosts);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,PublishedDate,ModifiedDate,Author,Image")] BlogPosts blogPosts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogPosts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPosts);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPosts = await _context.BlogPosts.FindAsync(id);
            if (blogPosts == null)
            {
                return NotFound();
            }
            return View(blogPosts);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PublishedDate,ModifiedDate,Author,Image")] BlogPosts blogPosts)
        {
            if (id != blogPosts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPosts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostsExists(blogPosts.Id))
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
            return View(blogPosts);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPosts = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPosts == null)
            {
                return NotFound();
            }

            return View(blogPosts);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPosts = await _context.BlogPosts.FindAsync(id);
            if (blogPosts != null)
            {
                _context.BlogPosts.Remove(blogPosts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostsExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }

        public IActionResult SingleBlog()
        {
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }
    }
}
