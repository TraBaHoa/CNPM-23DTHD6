using System;
using System.Collections.Generic;

namespace MedRateSystem.Models;

public partial class BenhNhan
{
    public string MaBenhNhan { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public virtual ICollection<PhieuKhaoSat> PhieuKhaoSats { get; set; } = new List<PhieuKhaoSat>();
}
