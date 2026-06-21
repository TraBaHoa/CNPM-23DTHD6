using MedRateSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedRateSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly MedRateContext _context;

        public AccountController(MedRateContext context) { _context = context; }

        // Trang đăng nhập cho Bác sĩ (Admin)
        [HttpGet]
        public IActionResult LoginAdmin() => View();

        [HttpPost]
        public IActionResult LoginAdmin(string taiKhoan, string matKhau)
        {
            var admin = _context.BacSi.FirstOrDefault(b => b.TaiKhoan == taiKhoan && b.MatKhau == matKhau);
            if (admin != null)
            {
                HttpContext.Session.SetString("MaUser", admin.MaBacSi);
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Dashboard", "Admin");
            }
            ViewBag.Error = "Tài khoản hoặc mật khẩu bác sĩ không hợp lệ!";
            return View();
        }
    }
}
