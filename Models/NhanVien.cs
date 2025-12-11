using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAPTOP.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        [Key]
        [StringLength(20)]
        public string MaNv { get; set; } = null!;

        [Column(TypeName = "nvarchar(100)")]
        public string? TenNv { get; set; }

        [StringLength(20)]
        public string? Sdt { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? QueQuan { get; set; }

       
        [Column(TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string? DiaChi { get; set; }
       

        public bool? GioiTinh { get; set; }
        public decimal? HeSoLuong { get; set; }
        public decimal? Luong { get; set; }

        // Login
        [Column(TypeName = "nvarchar(50)")]
        public string? UserName { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? PasswordHash { get; set; }

       
        [Column(TypeName = "nvarchar(20)")]
        public string? Role { get; set; } // admin / staff
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}