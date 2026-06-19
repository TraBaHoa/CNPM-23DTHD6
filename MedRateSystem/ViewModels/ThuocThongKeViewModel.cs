namespace MedRateSystem.ViewModels
{
    public class ThuocThongKeViewModel
    {
        public int SoLuong { get; set; }
        public string MaThuoc { get; set; } = string.Empty;
        public string TenThuoc { get; set; } = string.Empty;
        public string NhaSanXuat { get; set; } = string.Empty;

        public double DiemLikertTB { get; set; }
        public double TyLeADR { get; set; }
        public int TongSoLuotDanhGia { get; set; }
    }
}