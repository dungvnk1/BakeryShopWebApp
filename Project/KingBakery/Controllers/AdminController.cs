using KingBakery.Data;
using KingBakery.Models;
using KingBakery.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingBakery.Controllers
{
    public class AdminController : Controller
    {
        private readonly KingBakeryContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(KingBakeryContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult FinManage()
        {
            return View();
        }

        public IActionResult SalaryManage()
        {
            var employees = _context.Employee
                .Select(e => new Employee
                {
                    UserID = e.UserID,
                    Salary = e.Salary,
                    HiredDate = e.HiredDate,
                    Status = e.Status,
                    Users = e.Users
                })
                .ToList();

            return View(employees);
        }

        [HttpPost]
        public JsonResult UpdateSalary(int userId, decimal salary)
        {
            _logger.LogInformation($"UpdateSalary called with userId: {userId}, salary: {salary}");
            try
            {
                var employee = _context.Employee.FirstOrDefault(e => e.UserID == userId);
                if (employee == null)
                {
                    _logger.LogWarning($"Employee not found with userId: {userId}");
                    return Json(new { success = false, message = "Employee not found." });
                }

                employee.Salary = (double)salary; // or decimal
                _context.SaveChanges();

                _logger.LogInformation($"Salary updated successfully for userId: {userId}");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary for userId: {userId}", userId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Search(string searchString)
        //{
        //    var users = await _context.Users
        //    .Where(u => u.FullName.Contains(searchString) || u.Username.Contains(searchString))
        //    .ToListAsync();

        //    var orders = await _context.Orders
        //        .Where(o => o.Status.Contains(searchString))
        //        .ToListAsync();

        //    var viewModel = new SearchResultViewModel
        //    {
        //        Users = users,
        //        Orders = orders,
        //        SearchString = searchString
        //    };

        //    if (users.Any())
        //    {
        //        return View("Users/Index", viewModel);
        //    }
        //    else if (orders.Any())
        //    {
        //        return View("Orders/Index", viewModel);
        //    }
        //    else
        //    {
        //        // Handle case where no results are found
        //        return View("NoResults");
        //    }
        //}

        // GET: Users/Details/5

        public async Task<IActionResult> UserDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.User = user;
            return View(user);
        }

        public async Task<IActionResult> CustomerManage()
        {
            UpdateCustomerRankings();

            var customer = await _context.Customer
                            .Join(_context.Users, // Join Customers with Users
                                customer => customer.UserID, // from the Customer table
                                user => user.ID, // join on User ID
                                (customer, user) => new CustomerDetailViewModel // project into the ViewModel
                                {
                                    CustomerID = customer.UserID,
                                    CusFullName = user.FullName,
                                    CusEmail = user.Email,
                                    CusPhoneNumber = user.PhoneNumber,
                                    CusRanking = customer.Ranking
                                })
                            .ToListAsync();

            return View(customer);
        }

        [HttpGet]
        public ActionResult GetDailyEarnings()
        {
            var earnings = _context.Orders
                             .Where(o => o.Status == "Đã giao hàng")
                             .GroupBy(o => o.DateTime.Value.Date)
                             .Select(g => new
                             {
                                 Date = g.Key,
                                 Total = g.Sum(o => o.TotalPrice)
                             })
                             .OrderBy(g => g.Date)
                             .ToList();

            return new JsonResult(earnings);
        }

        [HttpGet]
        public JsonResult GetOrderCounts()
        {
            var orderCounts = _context.Orders
                .GroupBy(o => o.DateTime.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();

            return new JsonResult(orderCounts);
        }

        [HttpGet]
        public JsonResult GetEarningsSummary()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyEarnings = _context.Orders
            .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Month == currentMonth && o.DateTime.Value.Year == currentYear)
            .Sum(o => o.TotalPrice);

            var annualEarnings = _context.Orders
                .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Year == currentYear)
                .Sum(o => o.TotalPrice);

            var monthlyOrderCount = _context.Orders
                .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Month == currentMonth && o.DateTime.Value.Year == currentYear)
                .Count();

            var annualOrderCount = _context.Orders
                .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Year == currentYear)
                .Count();

            return new JsonResult(new
            {
                MonthlyEarnings = monthlyEarnings,
                AnnualEarnings = annualEarnings,
                MonthlyOrderCount = monthlyOrderCount,
                AnnualOrderCount = annualOrderCount
            });
        }

        public JsonResult GetFinancialSummary()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var monthlyEarnings = _context.Orders
            .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Month == today.Month && o.DateTime.Value.Year == today.Year)
            .Sum(o => o.TotalPrice);

            var annualEarnings = _context.Orders
                .Where(o => o.Status == "Đã giao hàng" && o.DateTime.HasValue && o.DateTime.Value.Year == today.Year)
                .Sum(o => o.TotalPrice);

            var monthlyCosts = _context.Employee
                .Sum(e => e.Salary);

            var annualCosts = _context.Employee
                .Sum(e => e.Salary);

            var monthlyProfits = monthlyEarnings - monthlyCosts;
            var annualProfits = annualEarnings - annualCosts;

            return Json(new
            {
                monthlyEarnings,
                annualEarnings,
                monthlyCosts,
                annualCosts,
                monthlyProfits,
                annualProfits
            });
        }

        public JsonResult GetDailyFinancialData()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var dailyData = (from o in _context.Orders
                             where o.DateTime >= startOfMonth
                             group o by o.DateTime.Value.Date into g
                             select new
                             {
                                 date = g.Key,
                                 earnings = g.Sum(x => x.TotalPrice),
                                 costs = _context.Employee.Sum(e => e.Salary)
                             }).ToList();

            var result = dailyData.Select(d => new
            {
                d.date,
                d.earnings,
                d.costs,
                profits = d.earnings - d.costs
            });

            return Json(result);
        }

        public JsonResult GetYearlyFinancialData()
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);

            var yearlyData = (from o in _context.Orders
                              where o.DateTime >= startOfYear
                              group o by new { o.DateTime.Value.Year, o.DateTime.Value.Month } into g
                              select new
                              {
                                  date = new DateTime(g.Key.Year, g.Key.Month, 1),
                                  earnings = g.Sum(x => x.TotalPrice),
                                  costs = _context.Employee.Sum(e => e.Salary)
                              }).ToList();

            var result = yearlyData.Select(d => new
            {
                d.date,
                d.earnings,
                d.costs,
                profits = d.earnings - d.costs
            });

            return Json(result);
        }

        public void UpdateCustomerRankings()
        {
            // Assuming you have a DbSet<OrderItem> in your context
            var orderItemsWithCustomers = _context.OrderItem
                                            .Include(oi => oi.Orders) // Assuming each OrderItem has an Order
                                            .Where(oi => oi.Orders.Status == "Đã giao hàng")
                                            .ToList(); // Retrieve all OrderItems and their Orders

            // Project OrderItems by CustomerID and aggregate TotalPrice
            var customerTotalSpends = orderItemsWithCustomers
                                        .GroupBy(oi => oi.CustomerID)
                                        .Select(group => new
                                        {
                                            CustomerID = group.Key,
                                            TotalSpent = group.Sum(oi => oi.Orders.TotalPrice)
                                        })
                                        .ToList();

            foreach (var customerSpend in customerTotalSpends)
            {
                var customer = _context.Customer.Find(customerSpend.CustomerID); // Find each customer based on ID

                if (customer != null)
                {
                    // Update ranking based on TotalSpent
                    if (customerSpend.TotalSpent < 1000000)
                    {
                        customer.Ranking = "Đồng";
                    }
                    else if (customerSpend.TotalSpent >= 1000000 && customerSpend.TotalSpent <= 2000000)
                    {
                        customer.Ranking = "Bạc";
                    }
                    else
                    {
                        customer.Ranking = "Vàng";
                    }
                }
            }

            _context.SaveChanges(); // Save changes once at the end for efficiency
        }

        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
