using System.ComponentModel.DataAnnotations;

namespace MedRateSystem.Models
{
    public class BacSi
    {
        [Key]
        public string MaBacSi { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string TaiKhoan { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }
}