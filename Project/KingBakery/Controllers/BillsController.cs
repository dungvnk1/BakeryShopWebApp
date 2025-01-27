﻿using KingBakery.Data;
using KingBakery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace KingBakery.Controllers
{
    
    public class BillsController : Controller
    {
        private readonly KingBakeryContext _context;

        public BillsController(KingBakeryContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "2")]
        public IActionResult Index(int? page, DateTime? fromDate, DateTime? toDate)
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
                var item = _context.OrderItem.FirstOrDefault(o => o.CustomerID == uid && o.OrderID == order.ID);
                if (item != null)
                {
                    orders.Add(order);
                }
            }

            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.DateTime >= fromDate.Value).ToList();
                ViewBag.From = fromDate.Value.ToString("yyyy-MM-dd");
            }

            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.DateTime <= toDate.Value.AddDays(1)).ToList();
                ViewBag.To = toDate.Value.ToString("yyyy-MM-dd"); 
            }

            
           

            //Pagination
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var od = orders.OrderByDescending(o => o.DateTime).ToPagedList(pageNumber, pageSize);
            return View(od);
        }

        [Authorize(Roles = "1,2")]
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
            ViewBag.Note = order.Note;
            ViewBag.Payment = order.Payment;

            var voucherPercent = 0;
            if (order.VoucherID != null)
            {
                voucherPercent = _context.Vouchers.FirstOrDefault(v => v.VoucherID == order.VoucherID).VPercent;
            }
            ViewBag.VoucherPercent = voucherPercent;

            var staff = _context.Users.FirstOrDefault(u => u.ID == order.StaffID);
            var shipper = _context.Users.FirstOrDefault(u => u.ID == order.ShipperID);
            var voucher = _context.Vouchers.FirstOrDefault(v => v.VoucherID == order.VoucherID);
            string staffName = "Chưa có", shipperName = "Chưa có", code = "Không có";
            if (staff != null)
            {
                staffName = staff.FullName;
            }
            if (shipper != null)
            {
                shipperName = shipper.FullName;
            }
            if (voucher != null && voucher.Code != null)
            {
                code = voucher.Code;
            }

            ViewBag.Staff = staffName;
            ViewBag.Shipper = shipperName;
            ViewBag.Voucher = code;
            ViewData["Bakery"] = _context.Bakery.ToList();
            return View(items);
        }

        [Authorize(Roles = "2")]
        public IActionResult Delete(int id)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);
            foreach (var item in items)
            {
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                bakery.Quantity += item.Quantity;
                _context.BakeryOption.Update(bakery);
                _context.SaveChanges();
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "2")]
        public IActionResult Cancel(int id)
        {
            var items = _context.OrderItem.Include(o => o.BakeryOption).Where(o => o.OrderID == id).ToList();
            var order = _context.Orders.Find(id);
            foreach (var item in items)
            {
                var bakery = _context.BakeryOption.FirstOrDefault(b => b.ID == item.BakeryID);
                bakery.Quantity += item.Quantity;
                _context.BakeryOption.Update(bakery);
                _context.SaveChanges();
            }
            order.Status = "Đã huỷ";
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "1,2")]
        public JsonResult GetCancelReason(int id)
        {
            var mess = _context.Orders.FirstOrDefault(o => o.ID == id);
            var reason = "Xin lỗi quý khách, hiện tại shop không thể ship hàng. Mong quý khách thông cảm.";
            if (mess != null)
            {
                if (mess.DenyReason != null)
                {
                    reason = mess.DenyReason;
                }
            }
            return Json(new
            {
                reason
            });
        }
    }
}