using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class StockItemsTableUnitAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "StockItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "StockItems");
        }
    }
}
