using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomWeb1.DataAccess.Migrations
{
    public partial class selectedAddinCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "selected",
                table: "ShoppingKarts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selected",
                table: "ShoppingKarts");
        }
    }
}
