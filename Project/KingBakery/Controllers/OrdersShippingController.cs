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
    public class OrdersShippingController : Controller
    {
        private readonly KingBakeryContext _context;

        public OrdersShippingController(KingBakeryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            var sum = orders.Where(o => o.Status == "Đã giao hàng").Sum(o => o.TotalPrice);
            var count = orders.Count();
            var today = DateTime.Now.ToShortDateString();

            var rtd = orders.Where(o => {
                var day = o.DateTime.Value.ToShortDateString();
                return (day == today) && (o.Status == "Đã giao hàng");
            })
                            .Sum(o => o.TotalPrice);

            ViewBag.Revenue = sum;
            ViewBag.NumberOrders = count - 1;
            ViewBag.RToday = rtd;
            return View(orders);
        }

    }
}
