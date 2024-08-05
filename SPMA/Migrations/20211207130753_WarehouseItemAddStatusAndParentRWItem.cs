using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class WarehouseItemAddStatusAndParentRWItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentRWItemId",
                table: "WarehouseItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "WarehouseItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_ParentRWItemId",
                table: "WarehouseItems",
                column: "ParentRWItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_InProductionRWs_ParentRWItemId",
                table: "WarehouseItems",
                column: "ParentRWItemId",
                principalTable: "InProductionRWs",
                principalColumn: "InProductionRWId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_InProductionRWs_ParentRWItemId",
                table: "WarehouseItems");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_ParentRWItemId",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "ParentRWItemId",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "State",
                table: "WarehouseItems");
        }
    }
}
