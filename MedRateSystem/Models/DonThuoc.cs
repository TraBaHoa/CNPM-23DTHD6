using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedRateSystem.Models
{
    public partial class DonThuoc
    {
        [Key]
        public string MaDonThuoc { get; set; } = null!;

        public string? MaBenhNhan { get; set; }
        public DateTime NgayKeDon { get; set; }
        public string? BacSiKeDon { get; set; }

        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan? MaBenhNhanNavigation { get; set; }

        [ForeignKey("BacSiKeDon")]
        public virtual BacSi? BacSiNavigation { get; set; }

        public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();
    }
}