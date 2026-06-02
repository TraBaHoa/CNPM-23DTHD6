using System;
using System.ComponentModel.DataAnnotations;

namespace MedRateSystem.Models
{
    public partial class ChiTietDonThuoc
    {
        [Key]
        public int Id { get; set; }
        public string MaDonThuoc { get; set; } = null!;
        public string MaThuoc { get; set; } = null!;

        public virtual Thuoc Thuoc { get; set; } = null!;
        public int? SoLuong { get; set; }
        public string? CachDung { get; set; }

        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
    }
}