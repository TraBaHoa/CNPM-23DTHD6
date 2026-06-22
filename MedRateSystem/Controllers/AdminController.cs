using MedRateSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MedRateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedRateSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly MedRateContext _context;
        public AdminController(MedRateContext context) { _context = context; }

        // MÀN HÌNH DASHBOARD TRUNG TÂM CỦA BÁC SĨ (Đã nâng cấp ADR Log)
        public async Task<IActionResult> Dashboard()
        {
            // Kiểm tra session để đảm bảo tính bảo mật
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginAdmin", "Account");
            }

            // 1. Tự động khắc phục dữ liệu cũ: Nếu Điểm tác dụng phụ <= 3 thì tự động ghi nhận là Có Tác Dụng Phụ
            var oldData = await _context.ChiTietKhaoSats.Where(c => c.CoTacDungPhu == false && c.DiemTacDungPhu <= 3).ToListAsync();
            if (oldData.Any())
            {
                foreach(var c in oldData) {
                    c.CoTacDungPhu = true;
                    c.MoTaTrieuChung = string.IsNullOrEmpty(c.NhanXet) ? "Có dấu hiệu ADR" : c.NhanXet;
                }
                await _context.SaveChangesAsync();
            }

            // 2. Lấy dữ liệu thống kê (Tối ưu bằng LINQ GroupBy)
            var thongKeThuoc = await _context.ChiTietKhaoSats
                .GroupBy(c => new {
                    c.MaThuoc,
                    TenThuoc = c.MaThuocNavigation != null ? c.MaThuocNavigation.TenThuoc : "N/A",
                    NhaSanXuat = c.MaThuocNavigation != null ? c.MaThuocNavigation.NhaSanXuat : ""
                })
                .Select(g => new ThuocThongKeViewModel
                {
                    MaThuoc = g.Key.MaThuoc ?? "N/A",
                    TenThuoc = g.Key.TenThuoc ?? "N/A",
                    NhaSanXuat = g.Key.NhaSanXuat ?? "",
                    DiemLikertTB = Math.Round(g.Average(x => (double)x.DiemLikert), 1),
                    TyLeADR = Math.Round(g.Count(x => x.CoTacDungPhu == true) * 100.0 / g.Count(), 1),
                    TongSoLuotDanhGia = g.Count()
                })
                .ToListAsync();

            // 2. Lấy danh sách biến cố
            var danhSachBienCo = await _context.ChiTietKhaoSats
                .Where(c => c.CoTacDungPhu == true)
                .Select(c => new BienCoLamSangViewModel
                {
                    MaPhieu = c.MaPhieu,
                    TenThuoc = c.MaThuocNavigation != null ? c.MaThuocNavigation.TenThuoc : "N/A",
                    DiemLikert = c.DiemLikert,
                    MoTaTrieuChung = c.MoTaTrieuChung ?? "Không có mô tả"
                })
                .ToListAsync();

            ViewBag.DanhSachBienCo = danhSachBienCo;

            return View(thongKeThuoc);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");
            // Nếu không phải Admin thì chặn lại
            if (role != "Admin")
            {
                context.Result = RedirectToAction("LoginAdmin", "Account");
            }
        }

        public async Task<IActionResult> PhantichAbcVen()
        {
            // Sử dụng .AsNoTracking() để truy vấn nhanh hơn và tránh lỗi xung đột Tracker
            var danhSachThuoc = await _context.ChiTietDonThuocs
                .AsNoTracking()
                .Join(_context.Thuocs,
                      ct => ct.MaThuoc,
                      t => t.MaThuoc,
                      (ct, t) => new { ct, t })
                .GroupBy(x => new { x.t.MaThuoc, x.t.TenThuoc, x.t.GiaTien, x.t.PhanLoaiVen })
                .Select(g => new AbcVenViewModel
                {
                    MaThuoc = g.Key.MaThuoc ?? "N/A",
                    TenThuoc = g.Key.TenThuoc ?? "Chưa rõ",
                    // Ép kiểu tường minh cho SoLuong và GiaTien để tránh lỗi null
                    TongSoLuongKe = (int)(g.Sum(x => x.ct.SoLuong ?? 0)),
                    ThanhTien = (decimal)(g.Sum(x => x.ct.SoLuong ?? 0)) * (decimal)(g.Key.GiaTien ?? 0),
                    NhomVEN = g.Key.PhanLoaiVen ?? "N"
                })
                .OrderByDescending(x => x.ThanhTien)
                .ToListAsync();

            decimal tongChiPhiHeThong = danhSachThuoc.Sum(x => x.ThanhTien);
            decimal tichLuyChiPhi = 0;

            foreach (var item in danhSachThuoc)
            {
                if (tongChiPhiHeThong > 0)
                {
                    item.TyleChiPhi = (double)((item.ThanhTien / tongChiPhiHeThong) * 100);
                    tichLuyChiPhi += item.ThanhTien;
                    double tyLeTichLuy = (double)((tichLuyChiPhi / tongChiPhiHeThong) * 100);

                    // Gán nhóm ABC dựa trên tỷ lệ tích lũy
                    if (tyLeTichLuy <= 75) item.NhomABC = "A";
                    else if (tyLeTichLuy <= 90) item.NhomABC = "B";
                    else item.NhomABC = "C";
                }
            }
            return View(danhSachThuoc);
        }


        // --- THÊM VÀO AdminController.cs ---

        public IActionResult QuanLyBacSi()
        {
            var list = _context.BacSi.ToList(); // Gọi DbSet<BacSi>
            return View(list);
        }

        // Danh sách Bệnh nhân (Lấy từ bảng BenhNhan)
        public IActionResult QuanLyBenhNhan()
        {
            var list = _context.BenhNhans.ToList(); // Gọi DbSet<BenhNhan>
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> XoaBacSi(string id)
        {
            var bs = await _context.BacSi.FindAsync(id);
            if (bs != null) { _context.BacSi.Remove(bs); await _context.SaveChangesAsync(); }
            return RedirectToAction("QuanLyBacSi");
        }

        [HttpPost]
        public async Task<IActionResult> XoaBenhNhan(string id)
        {
            var bn = await _context.BenhNhans.FindAsync(id);
            if (bn != null) 
            { 
                // Xóa Chi Tiết Khảo Sát và Phiếu Khảo Sát
                var phieuKhaoSats = _context.PhieuKhaoSats.Where(p => p.MaBenhNhan == id).ToList();
                foreach(var p in phieuKhaoSats) {
                    var chiTietKS = _context.ChiTietKhaoSats.Where(c => c.MaPhieu == p.MaPhieu).ToList();
                    _context.ChiTietKhaoSats.RemoveRange(chiTietKS);
                }
                _context.PhieuKhaoSats.RemoveRange(phieuKhaoSats);

                // Xóa Chi Tiết Đơn Thuốc và Đơn Thuốc
                var donThuocs = _context.DonThuocs.Where(d => d.MaBenhNhan == id).ToList();
                foreach(var d in donThuocs) {
                    var chiTietDT = _context.ChiTietDonThuocs.Where(c => c.MaDonThuoc == d.MaDonThuoc).ToList();
                    _context.ChiTietDonThuocs.RemoveRange(chiTietDT);
                }
                _context.DonThuocs.RemoveRange(donThuocs);

                // Cuối cùng xóa Bệnh Nhân
                _context.BenhNhans.Remove(bn); 
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction("QuanLyBenhNhan");
        }

        // =========================================================
        // CÁC CLASS VIEWMODEL PHỤ PHỤC VỤ HIỂN THỊ DỮ LIỆU

        public async Task<IActionResult> LichSuDanhGia(string? id)
        {
            var query = _context.ChiTietKhaoSats
                .Include(c => c.MaThuocNavigation)
                .Include(c => c.MaPhieuNavigation)
                .ThenInclude(p => p.MaBenhNhanNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(c => c.MaPhieuNavigation.MaBenhNhan == id);
                ViewBag.TenBenhNhan = await _context.BenhNhans
                    .Where(b => b.MaBenhNhan == id)
                    .Select(b => b.HoTen)
                    .FirstOrDefaultAsync();
                ViewBag.MaBenhNhan = id;
            }

            var lichSu = await query.OrderByDescending(c => c.MaPhieuNavigation.ThoiGianLamPhieu).ToListAsync();
            return View(lichSu);
        }

        // 1. ViewModel dành cho bảng thống kê tổng quan
        // 2. ViewModel mới bổ sung dành cho bảng Nhật ký triệu chứng (ADR Log)
    }
}