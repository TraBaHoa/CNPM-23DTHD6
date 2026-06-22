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

        private void SetHeaderStats()
        {
            var data = _context.ChiTietKhaoSats.ToList();
            ViewBag.TongSoDanhGia = data.Count;
            // Tính trung bình của điểm tổng thể
            ViewBag.DiemTrungBinh = data.Any() ? data.Average(c => c.DiemTongThe) : 0;
        }

        // --- 1. KHỞI TẠO ---

        public async Task<IActionResult> Index()
        {
            string? maBenhNhan = HttpContext.Session.GetString("MaUser");

            if (string.IsNullOrEmpty(maBenhNhan)) return RedirectToAction("Login", "Auth");

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            if (benhNhan == null) return RedirectToAction("Login", "Auth");

            ViewBag.BenhNhan = benhNhan;
            SetHeaderStats();

            // Tự động thêm 10 loại thuốc mẫu nếu CSDL chưa có đủ
            var existingThuocs = await _context.Thuocs.Select(t => t.MaThuoc).ToListAsync();
            var listThuoc = new List<Thuoc> {
                new Thuoc { MaThuoc = "T01", TenThuoc = "Amlodipine 5mg", NhaSanXuat = "Pharma 1", GiaTien = 15000 },
                new Thuoc { MaThuoc = "T02", TenThuoc = "Metformin 500mg", NhaSanXuat = "Pharma 2", GiaTien = 20000 },
                new Thuoc { MaThuoc = "T03", TenThuoc = "Atorvastatin 20mg", NhaSanXuat = "Pharma 3", GiaTien = 35000 },
                new Thuoc { MaThuoc = "T04", TenThuoc = "Omeprazole 20mg", NhaSanXuat = "Pharma 4", GiaTien = 25000 },
                new Thuoc { MaThuoc = "T05", TenThuoc = "Paracetamol 500mg", NhaSanXuat = "Hau Giang", GiaTien = 5000 },
                new Thuoc { MaThuoc = "T06", TenThuoc = "Amoxicillin 500mg", NhaSanXuat = "Domesco", GiaTien = 12000 },
                new Thuoc { MaThuoc = "T07", TenThuoc = "Loratadine 10mg", NhaSanXuat = "Traphaco", GiaTien = 8000 },
                new Thuoc { MaThuoc = "T08", TenThuoc = "Vitamin C 500mg", NhaSanXuat = "OPC", GiaTien = 10000 },
                new Thuoc { MaThuoc = "T09", TenThuoc = "Salbutamol 2mg", NhaSanXuat = "Mekophar", GiaTien = 15000 },
                new Thuoc { MaThuoc = "T10", TenThuoc = "Ibuprofen 400mg", NhaSanXuat = "Sanofi", GiaTien = 18000 }
            };
            
            var thuocToAdd = listThuoc.Where(t => !existingThuocs.Contains(t.MaThuoc)).ToList();
            if (thuocToAdd.Any())
            {
                _context.Thuocs.AddRange(thuocToAdd);
                await _context.SaveChangesAsync();
            }

            var donThuoc = await _context.DonThuocs
                                         .Where(d => d.MaBenhNhan == maBenhNhan)
                                         .OrderByDescending(d => d.NgayKeDon)
                                         .FirstOrDefaultAsync();

            // Đảm bảo bệnh nhân có đơn thuốc
            if (donThuoc == null)
            {
                donThuoc = new DonThuoc {
                    MaDonThuoc = "DT_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    MaBenhNhan = maBenhNhan,
                    NgayKeDon = DateTime.Now
                };
                _context.DonThuocs.Add(donThuoc);
                await _context.SaveChangesAsync();
            }

            // Cập nhật đơn thuốc để chứa đủ 10 loại thuốc
            var existingChiTiet = await _context.ChiTietDonThuocs.Where(ct => ct.MaDonThuoc == donThuoc.MaDonThuoc).Select(ct => ct.MaThuoc).ToListAsync();
            var allThuoc = await _context.Thuocs.ToListAsync();
            foreach(var t in allThuoc)
            {
                if (!existingChiTiet.Contains(t.MaThuoc))
                {
                    _context.ChiTietDonThuocs.Add(new ChiTietDonThuoc {
                        MaDonThuoc = donThuoc.MaDonThuoc,
                        MaThuoc = t.MaThuoc,
                        SoLuong = 10,
                        CachDung = "Ngày 2 lần"
                    });
                }
            }
            await _context.SaveChangesAsync();

            List<Thuoc> danhSachThuoc = new List<Thuoc>();
            if (donThuoc != null)
            {
                danhSachThuoc = await _context.ChiTietDonThuocs
                                                .Where(ct => ct.MaDonThuoc == donThuoc.MaDonThuoc)
                                                .Select(ct => ct.Thuoc)
                                                .ToListAsync();
            }

            return View(danhSachThuoc);
        }

        // --- 2. XỬ LÝ GỬI ĐÁNH GIÁ (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(DanhGiaViewModel DanhGia)
        {
            string? maBenhNhan = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maBenhNhan)) return RedirectToAction("Login", "Auth");

            if (DanhGia == null || string.IsNullOrEmpty(DanhGia.MaThuoc)) return RedirectToAction("Index");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lưu vào phiếu khảo sát cùng với Tình trạng bệnh
                var phieu = new PhieuKhaoSat { 
                    MaBenhNhan = maBenhNhan, 
                    ThoiGianLamPhieu = DateTime.Now,
                    TinhTrangBenh = DanhGia.TinhTrangBenh,
                    GhiChuNhanXet = DanhGia.NhanXet 
                };
                _context.PhieuKhaoSats.Add(phieu);
                await _context.SaveChangesAsync();

                bool hasADR = DanhGia.DiemTacDungPhu <= 3;
                string? trieuChung = DanhGia.TacDungPhu != null && DanhGia.TacDungPhu.Any() 
                    ? string.Join(", ", DanhGia.TacDungPhu) 
                    : (hasADR ? DanhGia.NhanXet : null);
                
                _context.ChiTietKhaoSats.Add(new ChiTietKhaoSat
                {
                    MaPhieu = phieu.MaPhieu,
                    MaThuoc = DanhGia.MaThuoc,
                    DiemLikert = DanhGia.DiemTongThe, 
                    DiemTongThe = DanhGia.DiemTongThe,
                    DiemHieuQua = DanhGia.DiemHieuQua,
                    DiemTacDungPhu = DanhGia.DiemTacDungPhu,
                    DiemTienLoi = DanhGia.DiemTienLoi,
                    CoTacDungPhu = hasADR || !string.IsNullOrEmpty(trieuChung),
                    MoTaTrieuChung = trieuChung,
                    NhanXet = DanhGia.NhanXet
                });
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("LichSu");
            }
            catch { await transaction.RollbackAsync(); return RedirectToAction("Index"); }
        }

        // --- 3. PHẦN HIỂN THỊ LỊCH SỬ & THỐNG KÊ ---
        public async Task<IActionResult> LichSu()
        {
            string? maBenhNhan = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maBenhNhan)) return RedirectToAction("Login", "Auth");

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            ViewBag.BenhNhan = benhNhan;
            SetHeaderStats();

            // Truy vấn TẤT CẢ dữ liệu cho cộng đồng, bao gồm Phiếu khảo sát và Bệnh nhân
            var lichSuKhaoSat = await _context.ChiTietKhaoSats
                .Include(c => c.MaThuocNavigation)
                .Include(c => c.MaPhieuNavigation)
                    .ThenInclude(p => p.MaBenhNhanNavigation)
                .OrderByDescending(c => c.MaPhieuNavigation.ThoiGianLamPhieu)
                .ToListAsync();

            return View(lichSuKhaoSat);
        }
        
        public async Task<IActionResult> ThongKe()
        {
            string? maBenhNhan = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maBenhNhan)) return RedirectToAction("Login", "Auth");

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            ViewBag.BenhNhan = benhNhan;
            SetHeaderStats();

            var data = await _context.ChiTietKhaoSats.Include(c => c.MaThuocNavigation).ToListAsync();
            
            // Tính số lượng theo phân bố sao
            ViewBag.Sao5 = data.Count(c => c.DiemTongThe == 5);
            ViewBag.Sao4 = data.Count(c => c.DiemTongThe == 4);
            ViewBag.Sao3 = data.Count(c => c.DiemTongThe == 3);
            ViewBag.Sao2 = data.Count(c => c.DiemTongThe == 2);
            ViewBag.Sao1 = data.Count(c => c.DiemTongThe == 1);

            ViewBag.HieuQuaTB = data.Any() ? data.Average(c => c.DiemHieuQua) : 0;
            ViewBag.TienLoiTB = data.Any() ? data.Average(c => c.DiemTienLoi) : 0;

            // Nhóm dữ liệu cho bảng xếp hạng thuốc
            var thongKeThuoc = data.GroupBy(c => c.MaThuocNavigation.TenThuoc)
                .Select(g => new { 
                    TenThuoc = g.Key, 
                    DiemTB = Math.Round(g.Average(c => c.DiemTongThe), 1),
                    SoLuot = g.Count()
                })
                .OrderByDescending(x => x.DiemTB)
                .ThenByDescending(x => x.SoLuot)
                .Take(5)
                .ToList();

            ViewBag.ThuocDuocDanhGiaCao = thongKeThuoc;

            return View();
        }
    }
}