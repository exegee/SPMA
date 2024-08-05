using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class OrderBookWareListNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Warelist",
                table: "OrderBooks",
                newName: "WareList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WareList",
                table: "OrderBooks",
                newName: "Warelist");
        }
    }
}
