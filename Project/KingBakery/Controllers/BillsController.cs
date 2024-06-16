using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class BillsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BillsController(KingBakeryContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }

            var orders = new List<Orders>();
            foreach (var order in _context.Orders.ToList())
            {
                var item = _context.OrderItem.FirstOrDefault(o => o.CustomerID==uid && o.OrderID==order.ID);
                if (item != null)
                {
                    orders.Add(order);
                }
            }
            return View(orders);
        }
        public IActionResult Detail(int id) 
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userID != null)
            {
                uid = int.Parse(userID);
            }
            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Phone = order.PhoneNumber;
            ViewBag.Address = order.AdrDelivery;
            ViewBag.Status = order.Status;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(items);
        }
    }
}
