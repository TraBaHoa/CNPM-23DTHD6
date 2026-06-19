using MedRateSystem.Models;
using Microsoft.AspNetCore.Http; // Đừng quên dòng này để dùng Session
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MedRateSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly MedRateContext _context;
        public AuthController(MedRateContext context) { _context = context; }

        // GET: Đăng ký
        public IActionResult Register() => View();

        // POST: Đăng ký
        [HttpPost]
        public IActionResult Register(BenhNhan model)
        {
            if (_context.BenhNhans.Any(b => b.TaiKhoan == model.TaiKhoan))
            {
                ViewBag.Error = "Tài khoản đã tồn tại!";
                return View(model);
            }

            // Tự sinh mã bệnh nhân để tránh lỗi PK null
            model.MaBenhNhan = "BN" + DateTime.Now.ToString("yyMMddHHmmss");

            _context.BenhNhans.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        // GET: Đăng nhập
        public IActionResult Login() => View();

        // POST: Đăng nhập
        [HttpPost]
        public IActionResult Login(string taiKhoan, string matKhau)
        {
            var user = _context.BenhNhans.FirstOrDefault(u => u.TaiKhoan == taiKhoan && u.MatKhau == matKhau);
            if (user != null)
            {
                // SỬA LỖI TẠI ĐÂY: Role phải là "User", không phải "Us   er"
                HttpContext.Session.SetString("MaUser", user.MaBenhNhan);
                HttpContext.Session.SetString("Role", "User");

                return RedirectToAction("Index", "Khaosat");
            }
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // GET: Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}