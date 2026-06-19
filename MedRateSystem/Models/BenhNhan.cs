using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <--- CẦN DÒNG NÀY

namespace MedRateSystem.Models;

public partial class BenhNhan
{
    [Key] // Cần thêm dòng này để đánh dấu khóa chính nếu chưa có
    public string MaBenhNhan { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    [Required(ErrorMessage = "Tài khoản không được để trống")]
    public string TaiKhoan { get; set; } = string.Empty; // Gán giá trị mặc định để tránh lỗi null

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = string.Empty;

    public virtual ICollection<PhieuKhaoSat> PhieuKhaoSats { get; set; } = new List<PhieuKhaoSat>();
}