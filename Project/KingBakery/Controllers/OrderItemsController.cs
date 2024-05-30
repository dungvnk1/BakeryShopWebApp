﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using KingBakery.Extensions;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly KingBakeryContext _context;

        public OrderItemsController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        //public async Task<IActionResult> Index()
        //{
        //    var kingBakeryContext = _context.OrderItem.Include(o => o.BakeryOption).Include(o => o.Orders);
        //    return View(await kingBakeryContext.ToListAsync());
        //}

        //Show cart
        public IActionResult Index()
        {
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(Orders);
        }

        public List<OrderItem> Orders
        {
            get
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uid = 0;
                if(userID != null)
                {
                    uid = int.Parse(userID);
                }

                var data = _context.OrderItem.Include(o => o.BakeryOption)
                                             .Where(o => o.CustomerID == uid)
                                             .Where(o => o.OrderID == 0)
                                             .ToList();
                if(data == null)
                {
                    data = new List<OrderItem>();
                }
                return data;
            }
        }
        public IActionResult AddToCart(int id, int quantity)
        {
            var myCart = Orders;
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //int quan = quantity;//int.Parse(quantity);
            var item = myCart.FirstOrDefault(c => c.BakeryID == id);

            if(item == null)
            {
                var bakery = _context.BakeryOption.Include(b => b.Bakery).FirstOrDefault(b => b.ID == id);
                item = new OrderItem()
                {
                    BakeryID = bakery.ID,
                    CustomerID = userID==null ? 0:int.Parse(userID),
                    OrderID = 0,
                    Price = bakery.Price,
                    Quantity = quantity
                };
                _context.OrderItem.Add(item);
                _context.SaveChanges();
            }
            else
            {
                var orderItem = _context.OrderItem.Find(item.ID);
                orderItem.Quantity = quantity;
                _context.OrderItem.Update(orderItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem(int id)
        {
            var item = _context.OrderItem.Find(id);

            if (item != null)
            {
                _context.OrderItem.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.BakeryOption)
                .Include(o => o.Orders)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID");
            ViewData["BillID"] = new SelectList(_context.Orders, "ID", "ID");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BakeryID,BillID,Quantity,Price")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", orderItem.BakeryID);
            ViewData["BillID"] = new SelectList(_context.Orders, "ID", "ID", orderItem.OrderID);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", orderItem.BakeryID);
            ViewData["BillID"] = new SelectList(_context.Orders, "ID", "ID", orderItem.OrderID);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BakeryID,BillID,Quantity,Price")] OrderItem orderItem)
        {
            if (id != orderItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.ID))
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
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", orderItem.BakeryID);
            ViewData["BillID"] = new SelectList(_context.Orders, "ID", "ID", orderItem.OrderID);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.BakeryOption)
                .Include(o => o.Orders)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItem.Remove(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.ID == id);
        }
    }
}
