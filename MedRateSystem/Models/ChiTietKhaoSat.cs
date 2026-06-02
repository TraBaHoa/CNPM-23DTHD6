using System;
using System.Collections.Generic;

namespace MedRateSystem.Models;

public partial class ChiTietKhaoSat
{
    public int MaPhieu { get; set; }

    public string MaThuoc { get; set; } = null!;

    public int DiemLikert { get; set; }

    public bool? CoTacDungPhu { get; set; }

    public string? MoTaTrieuChung { get; set; }

    public virtual PhieuKhaoSat MaPhieuNavigation { get; set; } = null!;

    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
}
