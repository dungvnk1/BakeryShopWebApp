using Azure;
using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Diagnostics;
using System.Security.Claims;

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
            return View();
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
            .ToList();
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            PagedList<Bakery> lst = new PagedList<Bakery>(bakeries, pageNumber, pageSize);

            var categories = _context.Category.ToList();
            ViewData["Categories"] = categories;
            return View(lst);
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult ProductDetail()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }
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
