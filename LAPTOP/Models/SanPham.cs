using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <-- CẦN THÊM DÒNG NÀY

namespace LAPTOP.Models
{
    // ==========================================================
    // CLASS MỚI BẠN CẦN THÊM (vì đã tạo bảng LoaiSanPham)
    // Bạn có thể đặt class này trong file SanPham.cs
    // hoặc tạo file mới Models/LoaiSanPham.cs
    // ==========================================================
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

    // ==========================================================
    // CLASS SanPham ĐÃ ĐƯỢC CẬP NHẬT
    // ==========================================================
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        // --- CÁC THUỘC TÍNH CŨ VẪN DÙNG ---
        public string MaSp { get; set; } = null!;
        public string? TenSp { get; set; }
        public decimal? Gia { get; set; }

        // --- CÁC THUỘC TÍNH ĐÃ BỊ XÓA KHỎI DB ---
        // public string? Loai { get; set; }     // <-- Đã xóa, thay bằng MaLoai
        // public bool? ConHang { get; set; } // <-- Đã xóa, thay bằng SoLuongTon

        // --- CÁC THUỘC TÍNH MỚI TỪ DB ---
        public string? HinhAnh { get; set; }      // <-- Sửa lỗi CS1061
        public int SoLuongTon { get; set; }     // <-- Thêm từ script
        public decimal? GiaKhuyenMai { get; set; } // <-- Thêm từ script
        public int? MaLoai { get; set; }          // <-- Thêm từ script
        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; }

        // --- CÁC LIÊN KẾT (Relationships) ---
        public virtual ChiTietSanPham ChiTietSanPham { get; set; } = null!;
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        // Liên kết khóa ngoại mới đến LoaiSanPham
        public virtual LoaiSanPham? LoaiSanPham { get; set; }
    }
}