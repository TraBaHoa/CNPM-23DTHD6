using Microsoft.AspNetCore.Mvc;

namespace MedRateSystem.ViewModels
{
    // Đây mới là ViewModel chuẩn: chỉ chứa dữ liệu
    public class BienCoLamSangViewModel
    {
        public int MaPhieu { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public int DiemLikert { get; set; }
        public string MoTaTrieuChung { get; set; } = string.Empty;
    }
}
