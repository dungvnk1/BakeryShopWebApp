using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly KingBakeryContext _context;

        public FavouritesController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: Favourites
        public async Task<IActionResult> Index()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kingBakeryContext = _context.Favourite.Where(u => u.CustomerID == int.Parse(userID)).Include(f => f.BakeryOption).ThenInclude(f => f.Bakery).Include(f => f.Customer);
            return View(await kingBakeryContext.ToListAsync());
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourite
                .Include(f => f.BakeryOption)
                .Include(f => f.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "BakeryOptions", new { id = favourite.BakeryOption.ID });
        }

        // GET: Favourites/Create
        public IActionResult Create()
        {
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID");
            ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID");
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int id_pro)
        {

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favouriteCheck = _context.Favourite.Any(u => (u.CustomerID == int.Parse(userID)) && (u.BakeryID == id));

            if (!favouriteCheck)
            {
                Favourite favourite = null;
                if (ModelState.IsValid)
                {


                    favourite = new Favourite
                    {
                        BakeryID = id,
                        CustomerID = int.Parse(userID)

                    };
                    _context.Add(favourite);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Bakeries", new { id = id_pro });
                }
                if (favourite != null)
                {
                    ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", id);
                    ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID", int.Parse(userID));
                    return View(favourite);
                }
            }

            return RedirectToAction("Details", "Bakeries", new { id = id_pro });
        }

        // GET: Favourites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = _context.Favourite.Include(f => f.BakeryOption).ThenInclude(f => f.Bakery).FirstOrDefault(f => f.ID == id);
            if (favourite == null)
            {
                return NotFound();
            }
            var sizeList = await _context.BakeryOption
    .Where(bo => bo.BakeryID == favourite.BakeryOption.BakeryID)
    .Select(bo => new { bo.ID, bo.Size })
    .Distinct()
    .ToListAsync();

            ViewData["Size"] = new SelectList(sizeList, "ID", "Size", favourite.BakeryOption.ID);

            return View(favourite);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Favourite favourite)
        {
            if (id != favourite.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    var existingFavourite = await _context.Favourite
                        .Include(f => f.BakeryOption)
                        .FirstOrDefaultAsync(f => f.ID == id);

                    if (existingFavourite != null)
                    {
                        var duplicateFavourite = await _context.Favourite
                    .FirstOrDefaultAsync(f =>
                        f.ID != id &&
                        f.CustomerID == existingFavourite.CustomerID &&
                        f.BakeryID == favourite.BakeryOption.ID);

                        if (duplicateFavourite != null)
                        {
                            _context.Favourite.Remove(duplicateFavourite);
                        }

                        existingFavourite.BakeryID = favourite.BakeryOption.ID;

                        _context.Update(existingFavourite);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavouriteExists(favourite.ID))
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
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", favourite.BakeryID);
            ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID", favourite.CustomerID);
            return View(favourite);
        }

        // GET: Favourites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourite
                .Include(f => f.BakeryOption)
                .Include(f => f.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return View(favourite);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favourite = await _context.Favourite.FindAsync(id);
            if (favourite != null)
            {
                _context.Favourite.Remove(favourite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouriteExists(int id)
        {
            return _context.Favourite.Any(e => e.ID == id);
        }
    }
}
