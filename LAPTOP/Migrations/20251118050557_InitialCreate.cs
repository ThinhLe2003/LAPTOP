using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoaiSanPhams",
                table: "LoaiSanPhams");

            migrationBuilder.RenameTable(
                name: "LoaiSanPhams",
                newName: "LoaiSanPham");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoaiSanPham",
                table: "LoaiSanPham",
                column: "MaLoai");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoaiSanPham",
                table: "LoaiSanPham");

            migrationBuilder.RenameTable(
                name: "LoaiSanPham",
                newName: "LoaiSanPhams");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoaiSanPhams",
                table: "LoaiSanPhams",
                column: "MaLoai");
        }
    }
}
