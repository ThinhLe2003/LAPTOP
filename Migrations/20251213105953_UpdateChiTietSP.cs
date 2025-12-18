using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class UpdateChiTietSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SDT = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    QueQuan = table.Column<string>(type: "nvarchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__2725CF1E3F3DD519", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "LoaiSanPham",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiSanPham", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    TenNV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SDT = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    QueQuan = table.Column<string>(type: "nvarchar(max)", unicode: false, maxLength: 50, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    HeSoLuong = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Luong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__2725D70A8F9AB923", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSP = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    GiaKhuyenMai = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaLoai = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SanPham__2725081CA3079783", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK_SanPham_LoaiSanPham",
                        column: x => x.MaLoai,
                        principalTable: "LoaiSanPham",
                        principalColumn: "MaLoai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    NgayLap = table.Column<DateTime>(type: "date", nullable: true),
                    MaNV = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: true),
                    MaKH = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__2725A6E0F36B725F", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK__HoaDon__MaKH__412EB0B6",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__HoaDon__MaNV__403A8C7D",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietSanPham",
                columns: table => new
                {
                    MaSP = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    Cpu = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Ram = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Vga = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    ManHinh = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    KichThuoc = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    MauSac = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    HeDieuHanh = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietS__2725081C6CDEC7DD", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK__ChiTietSan__MaSP__3D5E1FD2",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon",
                columns: table => new
                {
                    MaHD = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, nullable: false),
                    MaSP = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietH__F557F661D651EF0C", x => new { x.MaHD, x.MaSP });
                    table.ForeignKey(
                        name: "FK__ChiTietHoa__MaHD__440B1D61",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                    table.ForeignKey(
                        name: "FK__ChiTietHoa__MaSP__44FF419A",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.InsertData(
                table: "LoaiSanPham",
                columns: new[] { "MaLoai", "TenLoai" },
                values: new object[,]
                {
                    { 1, "Laptop Gaming" },
                    { 2, "Laptop Văn Phòng" },
                    { 3, "Macbook - Apple" },
                    { 4, "Laptop Đồ Họa" },
                    { 5, "Phụ Kiện" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaSP",
                table: "ChiTietHoaDon",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDon",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaNV",
                table: "HoaDon",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "UQ_KhachHang_Email",
                table: "KhachHang",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaLoai",
                table: "SanPham",
                column: "MaLoai");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietHoaDon");

            migrationBuilder.DropTable(
                name: "ChiTietSanPham");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "LoaiSanPham");
        }
    }
}
