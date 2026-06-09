using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedRateSystem.Models;

public partial class ChiTietKhaoSat
{
    [Key]
    public int MaPhieu { get; set; }
    public string MaThuoc { get; set; } = null!;
    public int DiemLikert { get; set; }
    public bool? CoTacDungPhu { get; set; }

    public string? MoTaTrieuChung { get; set; }

    public virtual PhieuKhaoSat MaPhieuNavigation { get; set; } = null!;
    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
    public int DiemTongThe { get; set; }
    public int DiemHieuQua { get; set; }
    public int DiemTacDungPhu { get; set; }
    public int DiemTienLoi { get; set; }
    public string? NhanXet { get; set; }
}