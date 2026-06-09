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
        public IActionResult Login(string maBenhNhan)
        {
            var benhNhan = _context.BenhNhans.FirstOrDefault(b => b.MaBenhNhan == maBenhNhan);
            if (benhNhan == null) { ViewBag.Error = "Mã không tồn tại!"; return View(); }
            return RedirectToAction("Index", new { id = maBenhNhan });
        }

        public async Task<IActionResult> Index(string id)
        {
            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == id);
            if (benhNhan == null) return RedirectToAction("Login");
            ViewBag.BenhNhan = benhNhan;

            var donThuoc = await _context.DonThuocs.Where(d => d.MaBenhNhan == id)
                                         .OrderByDescending(d => d.NgayKeDon).FirstOrDefaultAsync();

            List<Thuoc> danhSachThuoc = new List<Thuoc>();
            if (donThuoc != null)
            {
                var maThuocList = await _context.ChiTietDonThuocs.Where(ct => ct.MaDonThuoc == donThuoc.MaDonThuoc)
                                                .Select(ct => ct.MaThuoc).ToListAsync();
                danhSachThuoc = await _context.Thuocs.Where(t => maThuocList.Contains(t.MaThuoc)).ToListAsync();
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