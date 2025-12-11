using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần thêm thư viện này cho [Column]

namespace LAPTOP.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        [Key]
        [StringLength(50)] // Hoặc theo độ dài khóa chính trong DB của bạn
        public string MaKh { get; set; } = null!;

        [StringLength(50)]
        public string? TenKh { get; set; }

        [StringLength(20)]
        public string? Sdt { get; set; }

        
        [Column(TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string? QueQuan { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string? DiaChi { get; set; }
        

        public bool? GioiTinh { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}