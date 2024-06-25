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
using KingBakery.ViewModel;

namespace KingBakery.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly KingBakeryContext _context;

        public FeedbacksController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index(int id)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).ThenInclude(f=>f.Bakery).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var model = new FeedbackItemViewModel
            {
                OrderID = id,
                Items = items
            };
            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Phone = order.PhoneNumber;
            ViewBag.Address = order.AdrDelivery;
            ViewBag.Status = order.Status;
            ViewBag.HasFB = order.HasFB;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(model);
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).ThenInclude(f => f.Bakery).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var feedback = await _context.Feedback.Where(u => u.OrderItems.Orders.ID == id).ToListAsync();
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var model = new FeedbackItemViewModel
            {
                OrderID = order.ID,
                Items = items,
                Feedbacks = feedback
            };
            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Phone = order.PhoneNumber;
            ViewBag.Address = order.AdrDelivery;
            ViewBag.Status = order.Status;
            ViewBag.HasFB = order.HasFB;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(model);

           
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID");
            ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID");
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (var feedback in model.Feedbacks)
                {
                    var newFeedback = new Feedback
                    {
                        OrderID = feedback.OrderID,
                        ContentFB = feedback.ContentFB,
                        CustomerID = int.Parse(userID),
                        BakeryID = feedback.BakeryID
                    };

                    _context.Feedback.Add(newFeedback);
                }

                await _context.SaveChangesAsync();

                // Update OrderItem to indicate it has feedbacks
                var orderItem = await _context.Orders.FindAsync(model.OrderID);
                if (orderItem != null)
                {
                    orderItem.HasFB = true;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Bills", new {});
            }

            return RedirectToAction("Index", new { id = model.OrderID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyFeedback(FeedbackItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (var feedback in model.FeedbackResponses)
                {
                    if(feedback.ReplyContent != null)
                    {
                        var newFeedback = new FeedbackResponse
                        {

                            FeedbackID = feedback.FeedbackID,
                            ReplyContent = feedback.ReplyContent,                          
                            StaffID = int.Parse(userID)
                        };
                        _context.FeedbackResponse.Add(newFeedback);
                    }
                   

                    
                }

                await _context.SaveChangesAsync();

                // Update OrderItem to indicate it has feedbacks


                return RedirectToAction("Index", "Orders", new { });
            }
            return RedirectToAction("Index", "Orders", new {});
        }
        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", feedback.BakeryID);
            ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID", feedback.CustomerID);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CustomerID,BakeryID,ContentFB")] Feedback feedback)
        {
            if (id != feedback.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.ID))
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
            ViewData["BakeryID"] = new SelectList(_context.BakeryOption, "ID", "ID", feedback.BakeryID);
            ViewData["CustomerID"] = new SelectList(_context.Customer, "UserID", "UserID", feedback.CustomerID);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .Include(f => f.Bakery)
                .Include(f => f.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedback.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedback.Any(e => e.ID == id);
        }
    }
}
