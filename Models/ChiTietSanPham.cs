using System;
using System.ComponentModel.DataAnnotations;      
using System.ComponentModel.DataAnnotations.Schema;

namespace LAPTOP.Models
{
    public partial class ChiTietSanPham
    {
        [Key]
        public string MaSp { get; set; } = null!;

        [Column(TypeName = "nvarchar(255)")]
        public string? Cpu { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Ram { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Vga { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? ManHinh { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? KichThuoc { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? MauSac { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? HeDieuHanh { get; set; }

        [ForeignKey("MaSp")]
        public virtual SanPham? MaSpNavigation { get; set; } = null!;
    }
}