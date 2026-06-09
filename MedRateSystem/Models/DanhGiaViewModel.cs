namespace MedRateSystem.Models
{
    public class DanhGiaViewModel
    {
        // 1. Mã thuốc để biết bệnh nhân đang đánh giá thuốc nào
        public string MaThuoc { get; set; } = "";

        // 2. Điểm số (1-5)
        public int DiemLikert { get; set; }

        // 3. Danh sách các triệu chứng (nếu chọn nhiều checkbox)
        public List<string>? TacDungPhu { get; set; }
        public string ?HoTen { get; set; }
        public string ?TinhTrangBenh { get; set; }
        public string ?TenThuoc { get; set; }
        public int DiemTongThe { get; set; }
        public int DiemHieuQua { get; set; }
        public int DiemTacDungPhu { get; set; }
        public int DiemTienLoi { get; set; }
        public string ?NhanXet { get; set; }
    }
}