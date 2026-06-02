using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <--- Dòng quan trọng nhất
using MedRateSystem.Models;
using System.Linq;

namespace MedRateSystem.Controllers
{
    public class DonThuocController : Controller
    {
        private readonly MedRateContext _context;
        
        public DonThuocController(MedRateContext context) 
        { 
            _context = context; 
        }

        // Hiển thị danh sách đơn thuốc của một bệnh nhân
        public IActionResult Index(string maBenhNhan)
        {
            var donThuocs = _context.DonThuocs
                                    .Where(d => d.MaBenhNhan == maBenhNhan)
                                    .ToList();
            return View(donThuocs);
        }

        // Hiển thị chi tiết đơn thuốc
        public IActionResult Details(string id)
        {
            // Bây giờ .Include() sẽ hoạt động bình thường
            var donThuoc = _context.DonThuocs
                                   .Include(d => d.ChiTietDonThuocs)
                                   .ThenInclude(ct => ct.Thuoc)
                                   .FirstOrDefault(d => d.MaDonThuoc == id);
            
            if (donThuoc == null) return NotFound();
            
            return View(donThuoc);
        }

        public IActionResult Create()
        {
            ViewBag.DanhSachThuoc = _context.Thuocs.ToList(); // Lấy 54 loại thuốc
            ViewBag.BenhNhans = _context.BenhNhans.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult LuuDonThuoc(KeDonViewModel model)
        {
            var donThuoc = new DonThuoc
            {
                MaDonThuoc = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                MaBenhNhan = model.MaBenhNhan,
                NgayKeDon = DateTime.Now
            };
            _context.DonThuocs.Add(donThuoc);
            _context.SaveChanges(); // Lưu DonThuoc trước để có ID

            foreach (var item in model.DanhSachThuoc)
            {
                _context.ChiTietDonThuocs.Add(new ChiTietDonThuoc
                {
                    MaDonThuoc = donThuoc.MaDonThuoc,
                    MaThuoc = item.MaThuoc,
                    SoLuong = item.SoLuong,
                    CachDung = item.CachDung
                });
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}