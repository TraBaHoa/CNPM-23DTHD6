using System;
using System.ComponentModel.DataAnnotations;

namespace MedRateSystem.Models
{
    public partial class ChiTietDonThuoc
    {
        public string MaDonThuoc { get; set; } = null!;
        public string MaThuoc { get; set; } = null!;
        public int? SoLuong { get; set; }
        public string? CachDung { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("MaThuoc")]
        public virtual Thuoc Thuoc { get; set; } = null!;
    }
}