using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <-- CẦN THÊM DÒNG NÀY
using System.ComponentModel.DataAnnotations.Schema;

namespace LAPTOP.Models
{
    
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        [Key]
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }

    
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        [Key]
        public string MaSp { get; set; } = null!;
        public string? TenSp { get; set; }
        public decimal? Gia { get; set; }

        public string? HinhAnh { get; set; }     
        public int SoLuongTon { get; set; }    
        public decimal? GiaKhuyenMai { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int MaLoai { get; set; }
        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; }

        //Relationship
        public virtual ChiTietSanPham? ChiTietSanPham { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        //Foreign key relationship to LoaiSanPham
        public virtual LoaiSanPham? LoaiSanPham { get; set; }
    }
}