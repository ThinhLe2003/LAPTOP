using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class FixRemainingVarChar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CHỈ ĐỔI NHỮNG CỘT CÒN LÀ VARCHAR → NVARCHAR
            // Bảng KhachHang
            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "KhachHang",
                type: "nvarcha  r(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "QueQuan",
                table: "KhachHang",
                type: "nvarchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            // Bảng NhanVien (nếu còn)
            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NhanVien",
                type: "nvarchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "QueQuan",
                table: "NhanVien",
                type: "nvarchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
            // Bảng SanPham (nếu còn)
            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "SanPham",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");
        }

        

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
