using System;
using System.Collections.Generic;

namespace MedRateSystem.Models;

public partial class PhieuCanhBaoAdr
{
    public int MaCanhBao { get; set; }

    public string MaThuoc { get; set; } = null!;

    public DateTime? NgayPhatHien { get; set; }

    public double TyLeThucTe { get; set; }

    public string NoiDungCanhBao { get; set; } = null!;

    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
}


