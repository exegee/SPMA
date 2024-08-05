using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class AddWeraHouseItemAndReservedItemTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseItems",
                columns: table => new
                {
                    WarehouseItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ComponentId = table.Column<int>(nullable: false),
                    WareId = table.Column<int>(nullable: false),
                    ComponentQty = table.Column<int>(nullable: false),
                    ReservedQty = table.Column<int>(nullable: true),
                    WareQty = table.Column<decimal>(nullable: true),
                    WareQtySum = table.Column<decimal>(nullable: true),
                    AddedDate = table.Column<DateTime>(nullable: true),
                    AddedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseItems", x => x.WarehouseItemId);
                    table.ForeignKey(
                        name: "FK_WarehouseItems_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseItems_Wares_WareId",
                        column: x => x.WareId,
                        principalTable: "Wares",
                        principalColumn: "WareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservedItems",
                columns: table => new
                {
                    ReservedItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InProductionRWId = table.Column<int>(nullable: true),
                    WarehouseItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedItems", x => x.ReservedItemId);
                    table.ForeignKey(
                        name: "FK_ReservedItems_InProductionRWs_InProductionRWId",
                        column: x => x.InProductionRWId,
                        principalTable: "InProductionRWs",
                        principalColumn: "InProductionRWId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservedItems_WarehouseItems_WarehouseItemId",
                        column: x => x.WarehouseItemId,
                        principalTable: "WarehouseItems",
                        principalColumn: "WarehouseItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedItems_InProductionRWId",
                table: "ReservedItems",
                column: "InProductionRWId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservedItems_WarehouseItemId",
                table: "ReservedItems",
                column: "WarehouseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_ComponentId",
                table: "WarehouseItems",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_WareId",
                table: "WarehouseItems",
                column: "WareId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedItems");

            migrationBuilder.DropTable(
                name: "WarehouseItems");
        }
    }
}
