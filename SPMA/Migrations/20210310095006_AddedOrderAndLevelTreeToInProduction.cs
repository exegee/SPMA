using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class AddedOrderAndLevelTreeToInProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookLevelTree",
                table: "InProduction",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookOrderTree",
                table: "InProduction",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookLevelTree",
                table: "InProduction");

            migrationBuilder.DropColumn(
                name: "BookOrderTree",
                table: "InProduction");
        }
    }
}
