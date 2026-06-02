using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedRateSystem.Models
{
    public class KeDonViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        [Display(Name = "Bệnh nhân")]
        public string MaBenhNhan { get; set; } = null!;

        // Danh sách các thuốc được chọn trong đơn
        public List<ChiTietDonThuocInput> DanhSachThuoc { get; set; } = new List<ChiTietDonThuocInput>();
    }

    public class ChiTietDonThuocInput
    {
        [Required(ErrorMessage = "Vui lòng chọn thuốc")]
        public string MaThuoc { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 1000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập cách dùng")]
        public string CachDung { get; set; } = null!;
    }
}