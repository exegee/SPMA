using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class ComponentIsAdditionallyPurchasableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionallyPurchasable",
                table: "Components",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdditionallyPurchasable",
                table: "Components");
        }
    }
}
