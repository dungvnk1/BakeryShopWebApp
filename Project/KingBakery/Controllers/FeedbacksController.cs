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
    public class FeedbacksController : Controller
    {
        private readonly KingBakeryContext _context;

        public FeedbacksController(KingBakeryContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public IActionResult Index(int bid)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == bid).ToList();
            var stt = "true";
            if(bid == -1)
            {
                stt = "false";
            }

            ViewBag.BID = bid;
            ViewBag.Status = stt;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedbacks(int bid, List<FeedbackViewModel> feedbacks)
        {
            var order = _context.Orders.FirstOrDefault(o => o.ID == bid);
            foreach (var feedback in feedbacks)
            {
                var content = feedback.ContentFB;
                content = content?.Trim();
                if (content != null && content != "")
                {
                    var newFeedback = new Feedback
                    {
                        CustomerID = feedback.CustomerID,
                        BakeryID = feedback.BakeryID,
                        ContentFB = feedback.ContentFB,
                        Time = DateTime.Now,
                    };
                    _context.Feedback.Add(newFeedback);
                }
            }
            if(order != null && order.HasFB == false)
            {
                order.HasFB = true;
                _context.Orders.Update(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { bid = -1 });

        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedback.Any(e => e.ID == id);
        }
    }
}

public class FeedbackViewModel
{
    public int CustomerID { get; set; }
    public int BakeryID { get; set; }
    public string ContentFB { get; set; }
}