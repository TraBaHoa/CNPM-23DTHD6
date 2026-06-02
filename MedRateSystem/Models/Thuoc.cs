using System;
using System.Collections.Generic;

namespace MedRateSystem.Models;

public partial class Thuoc
{
    public decimal? GiaTien { get; set; }
    public string? PhanLoaiVen { get; set; }
    public string MaThuoc { get; set; } = null!;

    public string TenThuoc { get; set; } = null!;

    public string? NhaSanXuat { get; set; }

    public double? DiemLikertTb { get; set; }

    public double? TyLeAdr { get; set; }

    public virtual ICollection<ChiTietKhaoSat> ChiTietKhaoSats { get; set; } = new List<ChiTietKhaoSat>();

    public virtual ICollection<PhieuCanhBaoAdr> PhieuCanhBaoAdrs { get; set; } = new List<PhieuCanhBaoAdr>();
}
