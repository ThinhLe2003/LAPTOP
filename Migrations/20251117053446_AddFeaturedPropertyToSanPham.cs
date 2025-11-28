using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAPTOP.Migrations
{
    public partial class AddFeaturedPropertyToSanPham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "SanPham",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "SanPham");
        }
    }
}
