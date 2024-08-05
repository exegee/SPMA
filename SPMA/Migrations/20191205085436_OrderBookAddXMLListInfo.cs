using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class OrderBookAddXMLListInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "PlasmaInList",
                table: "OrderBooks",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PlasmaOutList",
                table: "OrderBooks",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PurchaseList",
                table: "OrderBooks",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Warelist",
                table: "OrderBooks",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlasmaInList",
                table: "OrderBooks");

            migrationBuilder.DropColumn(
                name: "PlasmaOutList",
                table: "OrderBooks");

            migrationBuilder.DropColumn(
                name: "PurchaseList",
                table: "OrderBooks");

            migrationBuilder.DropColumn(
                name: "Warelist",
                table: "OrderBooks");
        }
    }
}
