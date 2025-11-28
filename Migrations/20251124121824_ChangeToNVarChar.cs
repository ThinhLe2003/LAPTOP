using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class ChangeToNVarChar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chỉ đổi các cột varchar → nvarchar(MAX) – không tạo bảng mới
            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NhanVien",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "QueQuan",
                table: "NhanVien",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "TenNv",
                table: "NhanVien",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            // SanPham
            migrationBuilder.AlterColumn<string>(
                name: "TenSp",
                table: "SanPham",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "SanPham",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            // KhachHang
            migrationBuilder.AlterColumn<string>(
                name: "TenKH",
                table: "KhachHang",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "QueQuan",
                table: "KhachHang",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "KhachHang",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
        }

       

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_LoaiSanPham",
                table: "SanPham");

            migrationBuilder.AlterColumn<int>(
                name: "MaLoai",
                table: "SanPham",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_LoaiSanPham",
                table: "SanPham",
                column: "MaLoai",
                principalTable: "LoaiSanPham",
                principalColumn: "MaLoai");
        }
    }
}
