using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedRateSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedRateSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly MedRateContext _context;

        public AdminController(MedRateContext context)
        {
            _context = context;
        }

        // MÀN HÌNH DASHBOARD TRUNG TÂM CỦA BÁC SĨ (Đã nâng cấp ADR Log)
        public async Task<IActionResult> Dashboard()
        {
            // 1. Lấy danh sách tất cả các loại thuốc
            var danhSachThuoc = await _context.Set<Thuoc>().ToListAsync();

            // 2. Tạo một danh sách lưu trữ kết quả tính toán tổng quan
            var thongKeThuoc = new List<ThuocThongKeViewModel>();

            // 3. TRUY VẤN NÂNG CẤP: Lấy danh sách các ca gặp tác dụng phụ cụ thể để làm Nhật ký biến cố
            var danhSachBienCo = await _context.Set<ChiTietKhaoSat>()
                                               .Where(c => c.CoTacDungPhu == true)
                                               .Join(_context.Set<Thuoc>(),
                                                     cc => cc.MaThuoc,
                                                     t => t.MaThuoc,
                                                     (cc, t) => new BienCoLamSangViewModel
                                                     {
                                                         MaPhieu = cc.MaPhieu,
                                                         TenThuoc = t.TenThuoc,
                                                         DiemLikert = cc.DiemLikert,
                                                         MoTaTrieuChung = cc.MoTaTrieuChung ?? ""
                                                     })
                                               .ToListAsync();

            // Đưa danh sách biến cố vào ViewBag để truyền song song ra giao diện View
            ViewBag.DanhSachBienCo = danhSachBienCo;

            // 4. Vòng lặp tính toán thống kê tổng quan từng đầu thuốc
            foreach (var thuoc in danhSachThuoc)
            {
                var danhGiaThuoc = await _context.ChiTietKhaoSats // Đảm bảo gọi đúng DbContext
                                 .Where(c => c.MaThuoc == thuoc.MaThuoc)
                                 .ToListAsync();

                double diemTrungBinh = 5.0;
                double tyLeADR = 0.0;

                if (danhGiaThuoc.Any())
                {
                    // Công thức tính điểm Likert trung bình
                    diemTrungBinh = danhGiaThuoc.Average(c => c.DiemLikert);

                    // Công thức tính tỷ lệ ADR
                    int soCaADR = danhGiaThuoc.Count(c => c.CoTacDungPhu == true);
                    tyLeADR = ((double)soCaADR / danhGiaThuoc.Count) * 100;
                }

                thongKeThuoc.Add(new ThuocThongKeViewModel
                {
                    MaThuoc = thuoc.MaThuoc,
                    TenThuoc = thuoc.TenThuoc,
                    NhaSanXuat = thuoc.NhaSanXuat ?? "",
                    DiemLikertTB = Math.Round(diemTrungBinh, 1),
                    TyLeADR = Math.Round(tyLeADR, 1),
                    TongSoLuotDanhGia = danhGiaThuoc.Count
                });
            }

            return View(thongKeThuoc);
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
    }

    // =========================================================
    // CÁC CLASS VIEWMODEL PHỤ PHỤC VỤ HIỂN THỊ DỮ LIỆU

    // 1. ViewModel dành cho bảng thống kê tổng quan
    public class ThuocThongKeViewModel
    {
        public string MaThuoc { get; set; } = "";
        public string TenThuoc { get; set; } = "";
        public string NhaSanXuat { get; set; } = "";
        public double DiemLikertTB { get; set; }
        public double TyLeADR { get; set; }
        public int TongSoLuotDanhGia { get; set; }
    }

    // 2. ViewModel mới bổ sung dành cho bảng Nhật ký triệu chứng (ADR Log)
    public class BienCoLamSangViewModel
    {
        public int MaPhieu { get; set; }
        public string TenThuoc { get; set; } = "";
        public int DiemLikert { get; set; }
        public string MoTaTrieuChung { get; set; } = "";
    }
}