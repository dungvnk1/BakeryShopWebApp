using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KingBakery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly KingBakeryContext _context;

        public CheckoutController(KingBakeryContext context)
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
            var user = _context.Users.Find(uid);
            ViewBag.FullName = user.FullName;
            ViewBag.Email = user.Email;
            ViewBag.Phone = user.PhoneNumber;
            ViewBag.Address = user.Address;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(Order);
        }

        public List<OrderItem> Order
        {
            get
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uid = 0;
                if (userID != null)
                {
                    uid = int.Parse(userID);
                }

                var data = _context.OrderItem.Include(o => o.BakeryOption)
                                             .Where(o => o.CustomerID == uid)
                                             .Where(o => o.OrderID == 0)
                                             .ToList();
                if (data == null)
                {
                    data = new List<OrderItem>();
                }
                return data;
            }
        }

        [HttpPost]
        public IActionResult CreateBill(string address, string number)
        {
            var bill = new Orders()
            {
                DateTime = DateTime.Now,
                AdrDelivery = address,
                PhoneNumber = number,
                Status = "Đã đặt hàng",
            };
            _context.Orders.Add(bill);
            _context.SaveChanges();

            double total = 0;
            foreach (var item in Order) 
            {
                var temp = _context.OrderItem.FirstOrDefault(o => o.ID == item.ID);
                temp.OrderID = bill.ID;
                total += temp.Price*temp.Quantity;
                _context.OrderItem.Update(temp);
                _context.SaveChanges();
            }
            bill.TotalPrice = total+20000;
            _context.Orders.Update(bill);
            _context.SaveChanges();

            return Redirect("/Home");
        }
    }
}
