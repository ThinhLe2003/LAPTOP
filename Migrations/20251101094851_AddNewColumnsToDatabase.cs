using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class AddNewColumnsToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- BẮT ĐẦU COPY TỪ ĐÂY ---

            // Tạo bảng LoaiSanPham MỚI
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

            // Xóa 2 cột cũ khỏi SanPham
            migrationBuilder.DropColumn(
                name: "ConHang",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "Loai",
                table: "SanPham");

            // Thêm 4 cột mới vào SanPham
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "SanPham",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GiaKhuyenMai",
                table: "SanPham",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongTon",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaLoai",
                table: "SanPham",
                type: "int",
                nullable: true);

            // Thêm 2 cột mới vào KhachHang
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "KhachHang",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "KhachHang",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            // Thêm Ràng buộc Khóa ngoại (FK) cho SanPham
            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_LoaiSanPham",
                table: "SanPham",
                column: "MaLoai",
                principalTable: "LoaiSanPham",
                principalColumn: "MaLoai");

            // Thêm Ràng buộc UNIQUE cho KhachHang
            migrationBuilder.AddUniqueConstraint(
                name: "UQ_KhachHang_Email",
                table: "KhachHang",
                column: "Email");

            // --- KẾT THÚC COPY TẠI ĐÂY ---
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
