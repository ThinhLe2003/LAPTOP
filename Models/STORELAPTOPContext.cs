using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LAPTOP.Models
{
    public partial class STORELAPTOPContext : DbContext
    {
        public STORELAPTOPContext()
        {
        }

        public STORELAPTOPContext(DbContextOptions<STORELAPTOPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; } = null!;
        public virtual DbSet<ChiTietSanPham> ChiTietSanPhams { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<SanPham> SanPhams { get; set; } = null!;

        // <-- THÊM MỚI (1)
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; } = null!;


      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                
                entity.HasKey(e => new { e.MaHd, e.MaSp })
                    .HasName("PK__ChiTietH__F557F661D651EF0C");
                entity.ToTable("ChiTietHoaDon");
                entity.Property(e => e.MaHd)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaHD")
                    .IsFixedLength();
                entity.Property(e => e.MaSp)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaSP")
                    .IsFixedLength();
                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
                entity.HasOne(d => d.MaHdNavigation)
                    .WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaHd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietHoa__MaHD__440B1D61");
                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietHoa__MaSP__44FF419A");
            });

            modelBuilder.Entity<ChiTietSanPham>(entity =>
            {

                
                entity.HasKey(e => e.MaSp)
                    .HasName("PK__ChiTietS__2725081C6CDEC7DD");
                entity.ToTable("ChiTietSanPham");
                entity.Property(e => e.MaSp)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaSP")
                    .IsFixedLength();
                entity.Property(e => e.Cpu)
                    .HasMaxLength(50)
                    .HasColumnName("CPU");
                entity.Property(e => e.HeDieuHanh).HasMaxLength(50);
                entity.Property(e => e.KichThuoc).HasMaxLength(50);
                entity.Property(e => e.ManHinh).HasMaxLength(50);
                entity.Property(e => e.MauSac).HasMaxLength(50);
                entity.Property(e => e.Ram)
                    .HasMaxLength(50)
                    .HasColumnName("RAM");
                entity.Property(e => e.Vga)
                    .HasMaxLength(50)
                    .HasColumnName("VGA");
                entity.HasOne(d => d.MaSpNavigation)
                    .WithOne(p => p.ChiTietSanPham)
                    .HasForeignKey<ChiTietSanPham>(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietSan__MaSP__3D5E1FD2");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
               
                entity.HasKey(e => e.MaHd)
                    .HasName("PK__HoaDon__2725A6E0F36B725F");
                entity.ToTable("HoaDon");
                entity.Property(e => e.MaHd)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaHD")
                    .IsFixedLength();
                entity.Property(e => e.MaKh)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH")
                    .IsFixedLength();
                entity.Property(e => e.MaNv)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaNV")
                    .IsFixedLength();
                entity.Property(e => e.NgayLap).HasColumnType("date");
                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK__HoaDon__MaKH__412EB0B6");
                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaNv)
                    .HasConstraintName("FK__HoaDon__MaNV__403A8C7D");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                
                entity.HasKey(e => e.MaKh)
                    .HasName("PK__KhachHan__2725CF1E3F3DD519");

                entity.ToTable("KhachHang");

               
                entity.HasIndex(e => e.Email).HasName("UQ_KhachHang_Email").IsUnique();

                entity.Property(e => e.MaKh)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MaKH")
                    .IsFixedLength();

                
                entity.Property(e => e.DiaChi)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.QueQuan)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Sdt)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength();
                entity.Property(e => e.TenKh)
                    .HasMaxLength(50)
                    .HasColumnName("TenKH");

                
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                
                entity.HasKey(e => e.MaNv)
                    .HasName("PK__NhanVien__2725D70A8F9AB923");
                entity.ToTable("NhanVien");
                entity.Property(e => e.MaNv)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaNV")
                    .IsFixedLength();
                entity.Property(e => e.DiaChi)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.HeSoLuong).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Luong).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.QueQuan)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Sdt)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength();
                entity.Property(e => e.TenNv)
                    .HasMaxLength(50)
                    .HasColumnName("TenNV");
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                
                entity.HasKey(e => e.MaSp)
                    .HasName("PK__SanPham__2725081CA3079783");

                entity.ToTable("SanPham");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("MaSP")
                    .IsFixedLength();

                
                entity.Property(e => e.HinhAnh).HasMaxLength(255);
                entity.Property(e => e.GiaKhuyenMai).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SoLuongTon);
                entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TenSp)
                    .HasMaxLength(50)
                    .HasColumnName("TenSP");

                
                entity.HasOne(d => d.LoaiSanPham)
                      .WithMany(p => p.SanPhams)
                      .HasForeignKey(d => d.MaLoai)
                      .HasConstraintName("FK_SanPham_LoaiSanPham");
                modelBuilder.Entity<LoaiSanPham>().HasData(
        new LoaiSanPham { MaLoai = 1, TenLoai = "Laptop Gaming" },
        new LoaiSanPham { MaLoai = 2, TenLoai = "Laptop Văn Phòng" },
        new LoaiSanPham { MaLoai = 3, TenLoai = "Macbook - Apple" },
        new LoaiSanPham { MaLoai = 4, TenLoai = "Laptop Đồ Họa" },
        new LoaiSanPham { MaLoai = 5, TenLoai = "Phụ Kiện" }
    );
            });

            
            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoai);
                entity.ToTable("LoaiSanPham");
                entity.Property(e => e.TenLoai).HasMaxLength(100);
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}