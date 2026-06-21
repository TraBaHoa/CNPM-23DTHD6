using System;
using System.Collections.Generic;

namespace MedRateSystem.Models;

public partial class PhieuKhaoSat
{
    public int MaPhieu { get; set; }

    public string MaBenhNhan { get; set; } = null!;

    public DateTime? ThoiGianLamPhieu { get; set; }

    public string? TinhTrangBenh { get; set; }

    public string? GhiChuNhanXet { get; set; }

    public virtual ICollection<ChiTietKhaoSat> ChiTietKhaoSats { get; set; } = new List<ChiTietKhaoSat>();

    public virtual BenhNhan MaBenhNhanNavigation { get; set; } = null!;
}
