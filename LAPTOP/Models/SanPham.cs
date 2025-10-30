using System;
using System.Collections.Generic;

namespace LAPTOP.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        public string MaSp { get; set; } = null!;
        public string? TenSp { get; set; }
        public string? Loai { get; set; }
        public decimal? Gia { get; set; }
        public bool? ConHang { get; set; }

        public virtual ChiTietSanPham ChiTietSanPham { get; set; } = null!;
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
