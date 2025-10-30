using System;
using System.Collections.Generic;

namespace LAPTOP.Models
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        public string MaHd { get; set; } = null!;
        public DateTime? NgayLap { get; set; }
        public string? MaNv { get; set; }
        public string? MaKh { get; set; }

        public virtual KhachHang? MaKhNavigation { get; set; }
        public virtual NhanVien? MaNvNavigation { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
