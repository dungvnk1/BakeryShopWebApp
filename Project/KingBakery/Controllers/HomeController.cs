using Azure;
using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Diagnostics;
using System.Security.Claims;
using KingBakery.ViewModel;
using System.Net.Mail;
using System.Net;

namespace KingBakery.Controllers
{
    public class HomeController : Controller
    {
        private readonly KingBakeryContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, KingBakeryContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userId != null)
            {
                uid = Int32.Parse(userId);
            }

            var cartQuantity = _context.OrderItem.Where(o => o.OrderID == 0 && o.CustomerID == uid).Count();
            HttpContext.Session.SetString("CartQuantity", cartQuantity.ToString());
            return View();
        }
        public async Task<IActionResult> Vouchers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uid = 0;
            if (userId != null)
            {
                uid = Int32.Parse(userId);
            }
            DateTime currentDate = DateTime.Now;
            return View(await _context.Vouchers.Include(v => v.Users)
                .Where(v => (v.UserID == null || v.UserID == uid) && v.Quantity > 0 && v.EndDate >= currentDate).ToListAsync());
        }
        public IActionResult Login()
        {
            return View("~/Views/Users/Login.cshtml");
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult ProductList(int? page)
        {
            var bakeries = _context.Bakery
            .Include(b => b.BakeryOptions)
            .Include(b => b.Category)

            .Where(b => b.isDeleted == false)
            .ToList();
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<Bakery> lst = new PagedList<Bakery>(bakeries, pageNumber, pageSize);

            //bestseller
            var bestSellingProduct = _context.OrderItem
                        .Include(oi => oi.Orders)
                        .Where(oi => oi.Orders != null && oi.Orders.Status == "Đã giao hàng")
                        .GroupBy(oi => oi.BakeryID)
                        .Select(group => new
                        {
                            BakeryID = group.Key,
                            TotalQuantity = group.Sum(oi => oi.Quantity)
                        })
                        .OrderByDescending(result => result.TotalQuantity)
                        .FirstOrDefault();
            if (bestSellingProduct != null)
                ViewData["bestseller"] = bestSellingProduct;

            //newest
            var newestProduct = _context.Bakery.OrderByDescending(b => b.ID).FirstOrDefault();
            if (newestProduct != null)
                ViewData["newestProduct"] = newestProduct;

            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(lst);
        }
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                var allAdmin = _context.Users.Where(a => a.Role == 3).ToList();
                foreach (var item in allAdmin)
                {
                    using (MailMessage message = new MailMessage("minhdangtai2422004@gmail.com", item.Email))
                    {
                        message.Subject = "[" + contact.Name + ": " + contact.Email + "]" + contact.Subject;
                        message.Body = contact.Message;
                        message.IsBodyHtml = false;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential network = new NetworkCredential("minhdangtai2422004@gmail.com", "bqha dxat cxdq xrjt");
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = network;
                            smtp.Port = 587;
                            smtp.Send(message);
                        }
                    }
                }
                TempData["success"] = "Send successfull!";
                return View("Contact", new ContactViewModel());
            }
            else
            {
                return View("Contact", contact);
            }
        }
        public IActionResult ProductDetail()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        //public IActionResult Blog()
        //{
        //    return View();
        //}
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Confirmation()
        {
            return View();
        }
        public IActionResult Elements()
        {
            return View();
        }
        public IActionResult SingleBlog()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }


}
