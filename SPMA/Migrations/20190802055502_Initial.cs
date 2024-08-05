using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SPMA.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OfficeNumber = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<short>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    AddedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(nullable: false),
                    ContactFirstName = table.Column<string>(nullable: true),
                    ContactLastName = table.Column<string>(nullable: true),
                    ContactTitle = table.Column<string>(nullable: true),
                    ContactPhone1 = table.Column<string>(nullable: true),
                    ContactPhone2 = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Discount = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "ProductionSockets",
                columns: table => new
                {
                    ProductionSocketId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionSockets", x => x.ProductionSocketId);
                });

            migrationBuilder.CreateTable(
                name: "ProductionStates",
                columns: table => new
                {
                    ProductionStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductionStateCode = table.Column<short>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionStates", x => x.ProductionStateId);
                });

            migrationBuilder.CreateTable(
                name: "Wares",
                columns: table => new
                {
                    WareId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(3, 2)", nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    Converter = table.Column<decimal>(type: "decimal(10, 10)", nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    TwG_Kod = table.Column<string>(nullable: true),
                    TwG_Nazwa = table.Column<string>(nullable: true),
                    Mag_Nazwa = table.Column<string>(nullable: true),
                    Mag_Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wares", x => x.WareId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PlannedQty = table.Column<int>(nullable: false),
                    FinishedQty = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: true),
                    Type = table.Column<short>(nullable: false),
                    ClientName = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: true),
                    RequiredDate = table.Column<DateTime>(nullable: true),
                    ShippedDate = table.Column<DateTime>(nullable: true),
                    ShippingName = table.Column<string>(nullable: true),
                    ShippingAddress = table.Column<string>(nullable: true),
                    ShippingCity = table.Column<string>(nullable: true),
                    ShippingPostalCode = table.Column<string>(nullable: true),
                    ShippingRegion = table.Column<string>(nullable: true),
                    ShippingCountry = table.Column<string>(nullable: true),
                    ShippingTypeId = table.Column<int>(nullable: true),
                    AddedById = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    ComponentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    MaterialType = table.Column<string>(nullable: true),
                    Cost = table.Column<decimal>(type: "Money", nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ComponentType = table.Column<short>(nullable: false),
                    LastSourceType = table.Column<short>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    AddedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    WareId = table.Column<int>(nullable: true),
                    WareQuantity = table.Column<int>(nullable: true),
                    WareLength = table.Column<decimal>(nullable: true),
                    WareUnit = table.Column<string>(nullable: true),
                    LastTechnology = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_Components_Wares_WareId",
                        column: x => x.WareId,
                        principalTable: "Wares",
                        principalColumn: "WareId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderBooks",
                columns: table => new
                {
                    OrderBookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    PlannedQty = table.Column<int>(nullable: false),
                    FinishedQty = table.Column<int>(nullable: false),
                    AddedDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    AddedById = table.Column<int>(nullable: true),
                    ProductionState = table.Column<short>(nullable: false),
                    ProductionStateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBooks", x => x.OrderBookId);
                    table.ForeignKey(
                        name: "FK_OrderBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderBooks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderBooks_ProductionStates_ProductionStateId",
                        column: x => x.ProductionStateId,
                        principalTable: "ProductionStates",
                        principalColumn: "ProductionStateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookComponents",
                columns: table => new
                {
                    BookComponentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BookId = table.Column<int>(nullable: false),
                    ComponentId = table.Column<int>(nullable: false),
                    LastSourceType = table.Column<short>(nullable: true),
                    LastSourceTypeComment = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    WareId = table.Column<int>(nullable: true),
                    WareQuantity = table.Column<int>(nullable: true),
                    WareLength = table.Column<decimal>(nullable: true),
                    WareUnit = table.Column<string>(nullable: true),
                    LastTechnology = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookComponents", x => x.BookComponentId);
                    table.ForeignKey(
                        name: "FK_BookComponents_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookComponents_Wares_WareId",
                        column: x => x.WareId,
                        principalTable: "Wares",
                        principalColumn: "WareId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InProduction",
                columns: table => new
                {
                    InProductionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderBookId = table.Column<int>(nullable: false),
                    ComponentId = table.Column<int>(nullable: false),
                    PlannedQty = table.Column<int>(nullable: false),
                    BookQty = table.Column<int>(nullable: false),
                    FinishedQty = table.Column<int>(nullable: false),
                    ProductionSocketId = table.Column<int>(nullable: true),
                    ProductionStateId = table.Column<int>(nullable: false),
                    Technology = table.Column<string>(nullable: true),
                    SourceType = table.Column<short>(nullable: true),
                    WareId = table.Column<int>(nullable: true),
                    WareQuantity = table.Column<int>(nullable: true),
                    WareLength = table.Column<decimal>(nullable: true),
                    WareUnit = table.Column<string>(nullable: true),
                    ParentReservationId = table.Column<int>(nullable: true),
                    ReservedQty = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ProductionTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InProduction", x => x.InProductionId);
                    table.ForeignKey(
                        name: "FK_InProduction_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InProduction_OrderBooks_OrderBookId",
                        column: x => x.OrderBookId,
                        principalTable: "OrderBooks",
                        principalColumn: "OrderBookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InProduction_InProduction_ParentReservationId",
                        column: x => x.ParentReservationId,
                        principalTable: "InProduction",
                        principalColumn: "InProductionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InProduction_ProductionSockets_ProductionSocketId",
                        column: x => x.ProductionSocketId,
                        principalTable: "ProductionSockets",
                        principalColumn: "ProductionSocketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InProduction_ProductionStates_ProductionStateId",
                        column: x => x.ProductionStateId,
                        principalTable: "ProductionStates",
                        principalColumn: "ProductionStateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InProduction_Wares_WareId",
                        column: x => x.WareId,
                        principalTable: "Wares",
                        principalColumn: "WareId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InProductionRWs",
                columns: table => new
                {
                    InProductionRWId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InProductionId = table.Column<int>(nullable: false),
                    WareId = table.Column<int>(nullable: true),
                    WareQuantity = table.Column<int>(nullable: true),
                    WareLength = table.Column<decimal>(nullable: true),
                    WareUnit = table.Column<string>(nullable: true),
                    ToIssue = table.Column<int>(nullable: false),
                    PlannedToCut = table.Column<int>(nullable: false),
                    Issued = table.Column<int>(nullable: false),
                    TotalToIssue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InProductionRWs", x => x.InProductionRWId);
                    table.ForeignKey(
                        name: "FK_InProductionRWs_InProduction_InProductionId",
                        column: x => x.InProductionId,
                        principalTable: "InProduction",
                        principalColumn: "InProductionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InProductionRWs_Wares_WareId",
                        column: x => x.WareId,
                        principalTable: "Wares",
                        principalColumn: "WareId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ProductionStates",
                columns: new[] { "ProductionStateId", "Comment", "Name", "ProductionStateCode" },
                values: new object[] { 1, "Oczekujący na dodanie do produkcji", "Bezczynny", (short)1 });

            migrationBuilder.InsertData(
                table: "ProductionStates",
                columns: new[] { "ProductionStateId", "Comment", "Name", "ProductionStateCode" },
                values: new object[] { 2, "Oczekujący w kolejce do produkcji", "Oczekuje", (short)2 });

            migrationBuilder.InsertData(
                table: "ProductionStates",
                columns: new[] { "ProductionStateId", "Comment", "Name", "ProductionStateCode" },
                values: new object[] { 3, "Część kupna nie uwzględniana w produkcji", "Kupna", (short)100 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookComponents_BookId",
                table: "BookComponents",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComponents_ComponentId",
                table: "BookComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComponents_WareId",
                table: "BookComponents",
                column: "WareId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_OfficeNumber",
                table: "Books",
                column: "OfficeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Components_Number",
                table: "Components",
                column: "Number",
                unique: true,
                filter: "[Number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Components_WareId",
                table: "Components",
                column: "WareId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_ComponentId",
                table: "InProduction",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_OrderBookId",
                table: "InProduction",
                column: "OrderBookId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_ParentReservationId",
                table: "InProduction",
                column: "ParentReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_ProductionSocketId",
                table: "InProduction",
                column: "ProductionSocketId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_ProductionStateId",
                table: "InProduction",
                column: "ProductionStateId");

            migrationBuilder.CreateIndex(
                name: "IX_InProduction_WareId",
                table: "InProduction",
                column: "WareId");

            migrationBuilder.CreateIndex(
                name: "IX_InProductionRWs_InProductionId",
                table: "InProductionRWs",
                column: "InProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_InProductionRWs_WareId",
                table: "InProductionRWs",
                column: "WareId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBooks_BookId",
                table: "OrderBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBooks_OrderId",
                table: "OrderBooks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBooks_ProductionStateId",
                table: "OrderBooks",
                column: "ProductionStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Number",
                table: "Orders",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionStates_ProductionStateCode",
                table: "ProductionStates",
                column: "ProductionStateCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wares_Code",
                table: "Wares",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookComponents");

            migrationBuilder.DropTable(
                name: "InProductionRWs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "InProduction");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "OrderBooks");

            migrationBuilder.DropTable(
                name: "ProductionSockets");

            migrationBuilder.DropTable(
                name: "Wares");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductionStates");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
