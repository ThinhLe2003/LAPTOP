using System;
using System.Collections.Generic;

namespace LAPTOP.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public string MaNv { get; set; } = null!;
        public string? TenNv { get; set; }
        public string? Sdt { get; set; }
        
        public string? QueQuan { get; set; }
        
        public string? DiaChi { get; set; }
        public bool? GioiTinh { get; set; }
        public decimal? HeSoLuong { get; set; }
        public decimal? Luong { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
