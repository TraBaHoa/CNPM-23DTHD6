namespace MedRateSystem.Controllers
{
    public class AbcVenViewModel
    {
        public string MaThuoc { get; set; } = string.Empty;
        public string TenThuoc { get; set; } = string.Empty;
        public int TongSoLuongKe { get; set; }
        public decimal ThanhTien { get; set; }
        public double TyleChiPhi { get; set; }
        public string NhomABC { get; set; } = string.Empty;
        public string NhomVEN { get; set; } = string.Empty;
    }
}