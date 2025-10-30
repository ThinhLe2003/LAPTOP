using System;
using System.Collections.Generic;

namespace LAPTOP.Models
{
    public partial class ChiTietSanPham
    {
        public string MaSp { get; set; } = null!;
        public string? Ram { get; set; }
        public string? Vga { get; set; }
        public string? ManHinh { get; set; }
        public string? Cpu { get; set; }
        public string? KichThuoc { get; set; }
        public string? MauSac { get; set; }
        public string? HeDieuHanh { get; set; }

        public virtual SanPham MaSpNavigation { get; set; } = null!;
    }
}
