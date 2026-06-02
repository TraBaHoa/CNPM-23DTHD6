using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedRateSystem.Models;

namespace MedRateSystem.Controllers
{
    public class KhaosatController : Controller
    {
        private readonly MedRateContext _context;

        public KhaosatController(MedRateContext context) => _context = context;

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
            var donThuoc = await _context.DonThuocs
                                         .Where(d => d.MaBenhNhan == id)
                                         .OrderByDescending(d => d.NgayKeDon)
                                         .FirstOrDefaultAsync();
            ViewBag.DonThuoc = donThuoc;

            List<Thuoc> danhSachThuoc = new List<Thuoc>();
            if (donThuoc != null)
            {
                var maThuocList = await _context.ChiTietDonThuocs
                                                .Where(ct => ct.MaDonThuoc == donThuoc.MaDonThuoc)
                                                .Select(ct => ct.MaThuoc).ToListAsync();
                danhSachThuoc = await _context.Thuocs
                                              .Where(t => maThuocList.Contains(t.MaThuoc))
                                              .ToListAsync();
            }
            return View(danhSachThuoc);
        }

        [HttpPost]
        public async Task<IActionResult> GuiDanhGia(string maBenhNhan, List<DanhGiaViewModel> DanhSachDanhGia, string GhiChuNhanXet)
        {
            // 1. Tạo phiếu
            var phieu = new PhieuKhaoSat { MaBenhNhan = maBenhNhan, ThoiGianLamPhieu = DateTime.Now };
            _context.PhieuKhaoSats.Add(phieu);
            await _context.SaveChangesAsync();

            // 2. Lưu chi tiết và xử lý cảnh báo ADR
            // Sửa đoạn trong vòng lặp foreach ở GuiDanhGia
            foreach (var item in DanhSachDanhGia)
            {
                // Kiểm tra null an toàn trước khi join
                var danhSachTrieuChung = item.TacDungPhu != null ? string.Join(", ", item.TacDungPhu) : "Không";
                bool coADR = item.TacDungPhu != null && item.TacDungPhu.Count > 0;

                var chiTiet = new ChiTietKhaoSat
                {
                    MaPhieu = phieu.MaPhieu,
                    MaThuoc = item.MaThuoc,
                    DiemLikert = item.DiemLikert,
                    CoTacDungPhu = coADR,
                    MoTaTrieuChung = danhSachTrieuChung
                };
                _context.Set<ChiTietKhaoSat>().Add(chiTiet);

                if (coADR)
                {
                    var canhBao = new PhieuCanhBaoAdr
                    {
                        MaThuoc = item.MaThuoc,
                        // Sử dụng NoiDungCanhBao thay vì MoTa
                        NoiDungCanhBao = "Bệnh nhân báo cáo: " + danhSachTrieuChung,
                        // Sử dụng NgayPhatHien thay vì NgayCanhBao
                        NgayPhatHien = DateTime.Now
                    };
                    _context.Set<PhieuCanhBaoAdr>().Add(canhBao);
                }
            }

            await _context.SaveChangesAsync();

            // 3. Logic Gợi ý bổ sung (nếu điểm Likert thấp)
            if (DanhSachDanhGia.Any(x => x.DiemLikert <= 2))
            {
                TempData["Message"] = "Hệ thống đã ghi nhận đánh giá của bạn. Ý kiến của bạn sẽ được chuyển đến bác sĩ để cân nhắc thay đổi phác đồ.";
            }

            return RedirectToAction("CamOn");
        }

        [HttpGet]
        public IActionResult CamOn() => View();
    }
}