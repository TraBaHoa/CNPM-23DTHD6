using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedRateSystem.Models;
using System.Linq;

namespace MedRateSystem.Controllers
{
    public class KhaosatController : Controller
    {
        private readonly MedRateContext _context;

        public KhaosatController(MedRateContext context) => _context = context;

        // --- 1. PHẦN ĐĂNG NHẬP & INDEX ---
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string taiKhoan, string matKhau)
        {
            // Tìm bệnh nhân trong bảng BenhNhan
            var user = _context.BenhNhans.FirstOrDefault(b => b.TaiKhoan == taiKhoan && b.MatKhau == matKhau);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }

            // Lưu Session
            HttpContext.Session.SetString("MaUser", user.MaBenhNhan);
            HttpContext.Session.SetString("Role", "User"); // Gán cứng là User vì đây là bảng BenhNhan

            return RedirectToAction("Index", "Khaosat");
        }

        [HttpPost]
        public IActionResult LoginAdmin(string taiKhoan, string matKhau)
        {
            // Tìm bác sĩ trong bảng BacSi
            var bs = _context.BacSi.FirstOrDefault(b => b.TaiKhoan == taiKhoan && b.MatKhau == matKhau);

            if (bs == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }

            // Lưu Session
            HttpContext.Session.SetString("MaUser", bs.MaBacSi);
            HttpContext.Session.SetString("Role", "Admin"); // Gán cứng là Admin

            return RedirectToAction("Dashboard", "Admin");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Khaosat");
        }

        public async Task<IActionResult> Index()
        {
            // Lấy MaBenhNhan từ Session thay vì từ tham số URL
            string? maBenhNhan = HttpContext.Session.GetString("MaUser");

            // Nếu chưa đăng nhập hoặc session hết hạn, quay về trang đăng nhập
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("Login", "Khaosat");
            }

            // Lấy thông tin bệnh nhân
            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            if (benhNhan == null) return RedirectToAction("Login", "Khaosat");

            ViewBag.BenhNhan = benhNhan;

            // Tìm đơn thuốc mới nhất của bệnh nhân này
            var donThuoc = await _context.DonThuocs
                                         .Where(d => d.MaBenhNhan == maBenhNhan)
                                         .OrderByDescending(d => d.NgayKeDon)
                                         .FirstOrDefaultAsync();

            List<Thuoc> danhSachThuoc = new List<Thuoc>();
            if (donThuoc != null)
            {
                // Tối ưu: Lấy trực tiếp danh sách thuốc thông qua Navigation Property (nếu Model của bạn đã định nghĩa quan hệ)
                danhSachThuoc = await _context.ChiTietDonThuocs
                                                .Where(ct => ct.MaDonThuoc == donThuoc.MaDonThuoc)
                                                .Select(ct => ct.MaThuocNavigation) // Giả định MaThuocNavigation là thuộc tính định hướng trong EF
                                                .ToListAsync();
            }

            return View(danhSachThuoc);
        }

        // --- 2. XỬ LÝ GỬI ĐÁNH GIÁ (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(string maBenhNhan, List<DanhGiaViewModel> DanhSachDanhGia)
        {
            if (DanhSachDanhGia == null || !DanhSachDanhGia.Any()) return RedirectToAction("Index", new { id = maBenhNhan });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var phieu = new PhieuKhaoSat { MaBenhNhan = maBenhNhan, ThoiGianLamPhieu = DateTime.Now };
                _context.PhieuKhaoSats.Add(phieu);
                await _context.SaveChangesAsync();

                foreach (var item in DanhSachDanhGia.Where(x => x.MaThuoc != null))
                {
                    var trieuChung = item.TacDungPhu != null ? string.Join(", ", item.TacDungPhu) : null;
                    _context.ChiTietKhaoSats.Add(new ChiTietKhaoSat
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaThuoc = item.MaThuoc,
                        DiemLikert = item.DiemLikert,
                        CoTacDungPhu = !string.IsNullOrEmpty(trieuChung),
                        MoTaTrieuChung = trieuChung
                    });
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("LichSu");
            }
            catch { await transaction.RollbackAsync(); return RedirectToAction("Index", new { id = maBenhNhan }); }
        }

        // --- 3. PHẦN HIỂN THỊ LỊCH SỬ & THỐNG KÊ ---
        public async Task<IActionResult> LichSu(string maBenhNhan)
        {
            // 1. Kiểm tra nếu không có mã bệnh nhân thì lấy từ Session (nếu bạn có dùng Session)
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                // Hoặc báo lỗi hoặc chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // 2. Truy vấn dữ liệu
            var lichSuKhaoSat = await _context.ChiTietKhaoSats
                .Include(c => c.MaThuocNavigation) // Bắt buộc phải có Include để lấy tên thuốc
                .Where(c => c.MaPhieuNavigation.MaBenhNhan == maBenhNhan) // Kiểm tra lại cột MaBenhNhan ở đây
                .OrderByDescending(c => c.MaPhieu)
                .ToListAsync();

            // 3. Gán mã bệnh nhân vào ViewBag để trang View dùng lại
            ViewBag.MaBenhNhan = maBenhNhan;

            return View(lichSuKhaoSat);
        }
        public async Task<IActionResult> ThongKe()
        {
            var data = await _context.ChiTietKhaoSats.ToListAsync();
            ViewBag.TongSoLuot = data.Count;
            ViewBag.DiemTB = data.Any() ? Math.Round(data.Average(c => c.DiemLikert), 1) : 0;

            // Nhóm dữ liệu cho biểu đồ thống kê thuốc
            var thongKeThuoc = data.GroupBy(c => c.MaThuoc)
                .Select(g => new { TenThuoc = g.Key, DiemTB = g.Average(c => c.DiemLikert) }).ToList();

            return View(thongKeThuoc);
        }
    }
}