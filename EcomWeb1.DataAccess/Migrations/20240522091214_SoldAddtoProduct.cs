using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomWeb1.DataAccess.Migrations
{
    public partial class SoldAddtoProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCopiesSold",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCopiesSold",
                table: "Products");
        }
    }
}
