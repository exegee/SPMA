using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class InProductionRWBoolToSbyte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "IsAdditionallyPurchasable",
                table: "InProductionRWs",
                nullable: false,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAdditionallyPurchasable",
                table: "InProductionRWs",
                nullable: false,
                oldClrType: typeof(short));
        }
    }
}
