using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using KingBakery.Helper;
using Microsoft.AspNetCore.Authentication.Google;
using KingBakery.ViewModel;
using X.PagedList;

namespace KingBakery.Controllers
{
    public class UsersController : Controller
    {
        private readonly KingBakeryContext _context;
        private readonly EmailServices _emailService;

        public UsersController(KingBakeryContext context, EmailServices emailServices)
        {
            _context = context;
            _emailService = emailServices;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 8; // Số lượng item trên mỗi trang
            int pageNumber = (page ?? 1);

            var users = _context.Users.OrderBy(u => u.ID).ToPagedList(pageNumber, pageSize);

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var user = _context.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault<Users>();
            if (user == null || _context.Users == null)
            {
                ViewBag.LoginError = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                return View();
            }
            if(user.IsBanned == 1)
            {
                ViewBag.BanLogin = "Tài khoản của bạn đã bị chặn!";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
            };

            HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

            int uid = user.ID;

            var cartQuantity = _context.OrderItem.Where(o => o.OrderID == 0 && o.CustomerID == uid).Count();
            HttpContext.Session.SetString("CartQuantity", cartQuantity.ToString());

            return RedirectToAction("Index", "Home");
        }

        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var userCheck = _context.Users.Where(u => u.Email == email).FirstOrDefault<Users>();
            if (userCheck == null)
            {
                Users user = new Users()
                {
                    FullName = result.Principal.FindFirstValue(ClaimTypes.Name),
                    Username = email,
                    Password = "",
                    ConfirmPassword = "",
                    Address = "",
                    BirthDate = DateOnly.MinValue,
                    Email = email,
                    PhoneNumber = "",
                    Role = 2
                };
                userCheck = user;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userCheck.ID.ToString()),
                new Claim(ClaimTypes.Name, userCheck.FullName),
                new Claim(ClaimTypes.Role, userCheck.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var isAdmin = User.IsInRole("1");
            ViewBag.IsAdmin = isAdmin;
            ViewData["RoleSelectList"] = new SelectList(new[]
                {
                        new { Value = "1", Text = "Admin" },
                        new { Value = "2", Text = "Khách hàng" },
                        new { Value = "3", Text = "Nhân viên" },
                        new { Value = "4", Text = "Shipper" }
                    }, "Value", "Text");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FullName,Username,Password,ConfirmPassword,Address,BirthDate,Email,PhoneNumber,Role,VertificationCode")] Users users)
        {
            ModelState.Remove("VertificationCode");

            if (ModelState.IsValid)
            {
                // Check if the user is not logged in
                if (!User.Identity.IsAuthenticated && !User.IsInRole("1"))
                {
                    // Set the Role to 2 if the user is not logged in
                    users.Role = 2;
                }

                _context.Add(users);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tài khoản đã được tạo thành công!";
                return View(users);
            }
            return View(users);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            var verificationCode = CreateVerificationCode();

            var user = new Users()
            {
                FullName = model.FullName,
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.Birthdate,
                Role = 2,
                ConfirmPassword = model.ConfirmPassword,
                VertificationCode = verificationCode
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var subject = "Verify Your Email";
            var body = $"Your verification code is: {verificationCode}";
            _emailService.SendEmail(model.Email, subject, body);


            return RedirectToAction("ConfirmEmail", new { id = user.ID });
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewData["RoleSelectList"] = new SelectList(new[]
                {
                        new { Value = "1", Text = "Admin" },
                        new { Value = "2", Text = "Khách hàng" },
                        new { Value = "3", Text = "Nhân viên" },
                        new { Value = "4", Text = "Shipper" }
                    }, "Value", "Text");
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Username,Password,Address,BirthDate,Email,PhoneNumber,Role")] Users users)
        {
            if (id != users.ID)
            {
                return NotFound();
            }

            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tài khoản của bạn đã được cập nhật thành công!";
                    return View(users);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult ForgotPasswordEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPasswordEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(m => m.Email == email);
            if (user != null)
            {
                string code = CreateVerificationCode();

                user.VertificationCode = code;
                _context.Update(user);
                _context.SaveChanges();

                var subject = "Verify Your Email";
                var body = $"Your verification code is: {code}";
                _emailService.SendEmail(email, subject, body);

                return RedirectToAction("ConfirmForgotPassword", new { id = user.ID });
            }
            return NotFound();
        }

        public IActionResult ConfirmForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmForgotPassword(int id, string code)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (code != user.VertificationCode)
            {
                return NotFound();
            }
            TempData["ConfirmEmailSuccess"] = "Xác thực thành công! Vui lòng đăng nhập lại!";
            return RedirectToAction("ForgotPassword", new { id = user.ID });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(int id, ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.ID == id);
                user.Password = model.Password;
                _context.Update(user);
                _context.SaveChanges();

                TempData["ChangePasswordSuccess"] = "Mật khẩu của bạn đã thay đổi thành công! Vui lòng đăng nhập lại!";
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        private string CreateVerificationCode()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[4];
                rng.GetBytes(tokenData);

                int token = BitConverter.ToInt32(tokenData, 0);
                var code = Math.Abs(token % 1000000).ToString("D6");

                return code;
            }
        }

        public IActionResult ConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(int id, string code)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (code != user.VertificationCode)
            {
                return NotFound();
            }
            TempData["ConfirmEmailSuccess"] = "Xác thực thành công! Vui lòng đăng nhập lại!";
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.ID == model.ID);
                if (user.Password.Equals(model.OldPassword))
                {
                    user.Password = model.Password;
                    user.ConfirmPassword = model.ConfirmPassword;
                    _context.Update(user);
                    _context.SaveChanges();

                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["ChangePasswordSuccess"] = "Mật khẩu của bạn đã thay đổi thành công! Vui lòng đăng nhập lại!";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["OldPasswordError"] = "Mật khẩu hiện tại không chính xác! Vui lòng nhập lại!";
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult BanUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if(user != null)
            {
                user.IsBanned = 1;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult UnBanUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user != null)
            {
                user.IsBanned = 0;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
