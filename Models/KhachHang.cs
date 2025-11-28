using System;
using System.Collections.Generic;

namespace LAPTOP.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public string MaKh { get; set; } = null!;
        public string? TenKh { get; set; }
        public string? Sdt { get; set; }
        
        public string? QueQuan { get; set; }
      
        public string? DiaChi { get; set; }
        public bool? GioiTinh { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
