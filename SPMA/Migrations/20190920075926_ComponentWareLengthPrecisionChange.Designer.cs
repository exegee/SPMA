﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SPMA.Data;

namespace SPMA.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190920075926_ComponentWareLengthPrecisionChange")]
    partial class ComponentWareLengthPrecisionChange
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SPMA.Models.Orders.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddedById");

                    b.Property<int?>("ClientId");

                    b.Property<string>("ClientName");

                    b.Property<string>("Comment");

                    b.Property<int>("FinishedQty");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Number")
                        .IsRequired();

                    b.Property<DateTime?>("OrderDate");

                    b.Property<int>("PlannedQty");

                    b.Property<DateTime?>("RequiredDate");

                    b.Property<DateTime?>("ShippedDate");

                    b.Property<string>("ShippingAddress");

                    b.Property<string>("ShippingCity");

                    b.Property<string>("ShippingCountry");

                    b.Property<string>("ShippingName");

                    b.Property<string>("ShippingPostalCode");

                    b.Property<string>("ShippingRegion");

                    b.Property<int?>("ShippingTypeId");

                    b.Property<int?>("State");

                    b.Property<short>("Type");

                    b.HasKey("OrderId");

                    b.HasIndex("ClientId");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("SPMA.Models.Orders.OrderBook", b =>
                {
                    b.Property<int>("OrderBookId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddedById");

                    b.Property<DateTime?>("AddedDate");

                    b.Property<int>("BookId");

                    b.Property<string>("Comment");

                    b.Property<int>("FinishedQty");

                    b.Property<string>("Number")
                        .IsRequired();

                    b.Property<int>("OrderId");

                    b.Property<int>("PlannedQty");

                    b.Property<short>("ProductionState");

                    b.Property<int?>("ProductionStateId");

                    b.HasKey("OrderBookId");

                    b.HasIndex("BookId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductionStateId");

                    b.ToTable("OrderBooks");
                });

            modelBuilder.Entity("SPMA.Models.Production.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddedBy");

                    b.Property<string>("Author");

                    b.Property<string>("Comment");

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OfficeNumber")
                        .IsRequired();

                    b.Property<short>("Type");

                    b.HasKey("BookId");

                    b.HasIndex("OfficeNumber")
                        .IsUnique();

                    b.ToTable("Books");
                });

            modelBuilder.Entity("SPMA.Models.Production.BookComponent", b =>
                {
                    b.Property<int>("BookComponentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookId");

                    b.Property<int>("ComponentId");

                    b.Property<short?>("LastSourceType");

                    b.Property<string>("LastSourceTypeComment");

                    b.Property<string>("LastTechnology");

                    b.Property<int>("Level");

                    b.Property<int>("Order");

                    b.Property<int>("Quantity");

                    b.Property<int?>("WareId");

                    b.Property<decimal?>("WareLength");

                    b.Property<int?>("WareQuantity");

                    b.Property<string>("WareUnit");

                    b.HasKey("BookComponentId");

                    b.HasIndex("BookId");

                    b.HasIndex("ComponentId");

                    b.HasIndex("WareId");

                    b.ToTable("BookComponents");
                });

            modelBuilder.Entity("SPMA.Models.Production.Component", b =>
                {
                    b.Property<int>("ComponentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddedBy");

                    b.Property<string>("Author");

                    b.Property<string>("Comment");

                    b.Property<short>("ComponentType");

                    b.Property<decimal?>("Cost")
                        .HasColumnType("Money");

                    b.Property<string>("Description");

                    b.Property<short>("LastSourceType");

                    b.Property<string>("LastTechnology");

                    b.Property<string>("MaterialType");

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<string>("Number");

                    b.Property<int?>("WareId");

                    b.Property<decimal?>("WareLength")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int?>("WareQuantity");

                    b.Property<string>("WareUnit");

                    b.Property<float?>("Weight");

                    b.HasKey("ComponentId");

                    b.HasIndex("Number")
                        .IsUnique()
                        .HasFilter("[Number] IS NOT NULL");

                    b.HasIndex("WareId");

                    b.ToTable("Components");
                });

            modelBuilder.Entity("SPMA.Models.Production.InProduction", b =>
                {
                    b.Property<int>("InProductionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookQty");

                    b.Property<int>("ComponentId");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("FinishedQty");

                    b.Property<int>("OrderBookId");

                    b.Property<int?>("ParentReservationId");

                    b.Property<int>("PlannedQty");

                    b.Property<int?>("ProductionSocketId");

                    b.Property<int>("ProductionStateId");

                    b.Property<string>("ProductionTime");

                    b.Property<int?>("ReservedQty");

                    b.Property<short?>("SourceType");

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("Technology");

                    b.Property<int?>("WareId");

                    b.Property<decimal?>("WareLength");

                    b.Property<int?>("WareQuantity");

                    b.Property<string>("WareUnit");

                    b.HasKey("InProductionId");

                    b.HasIndex("ComponentId");

                    b.HasIndex("OrderBookId");

                    b.HasIndex("ParentReservationId");

                    b.HasIndex("ProductionSocketId");

                    b.HasIndex("ProductionStateId");

                    b.HasIndex("WareId");

                    b.ToTable("InProduction");
                });

            modelBuilder.Entity("SPMA.Models.Production.InProductionRW", b =>
                {
                    b.Property<int>("InProductionRWId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("InProductionId");

                    b.Property<int>("Issued");

                    b.Property<int>("PlannedToCut");

                    b.Property<int>("ToIssue");

                    b.Property<int>("TotalToIssue");

                    b.Property<int?>("WareId");

                    b.Property<decimal?>("WareLength");

                    b.Property<int?>("WareQuantity");

                    b.Property<string>("WareUnit");

                    b.HasKey("InProductionRWId");

                    b.HasIndex("InProductionId");

                    b.HasIndex("WareId");

                    b.ToTable("InProductionRWs");
                });

            modelBuilder.Entity("SPMA.Models.Production.ProductionSocket", b =>
                {
                    b.Property<int>("ProductionSocketId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<string>("Name");

                    b.HasKey("ProductionSocketId");

                    b.ToTable("ProductionSockets");
                });

            modelBuilder.Entity("SPMA.Models.Production.ProductionState", b =>
                {
                    b.Property<int>("ProductionStateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<string>("Name");

                    b.Property<short>("ProductionStateCode");

                    b.HasKey("ProductionStateId");

                    b.HasIndex("ProductionStateCode")
                        .IsUnique();

                    b.ToTable("ProductionStates");

                    b.HasData(
                        new { ProductionStateId = 1, Comment = "Oczekujący na dodanie do produkcji", Name = "Bezczynny", ProductionStateCode = (short)1 },
                        new { ProductionStateId = 2, Comment = "Oczekujący w kolejce do produkcji", Name = "Oczekuje", ProductionStateCode = (short)2 },
                        new { ProductionStateId = 3, Comment = "Część kupna nie uwzględniana w produkcji", Name = "Kupna", ProductionStateCode = (short)100 }
                    );
                });

            modelBuilder.Entity("SPMA.Models.Sales.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("Comment");

                    b.Property<string>("CompanyName")
                        .IsRequired();

                    b.Property<string>("ContactFirstName");

                    b.Property<string>("ContactLastName");

                    b.Property<string>("ContactPhone1");

                    b.Property<string>("ContactPhone2");

                    b.Property<string>("ContactTitle");

                    b.Property<string>("Country");

                    b.Property<int?>("Discount");

                    b.Property<string>("Email");

                    b.Property<string>("Fax");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Region");

                    b.HasKey("ClientId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("SPMA.Models.Warehouse.Ware", b =>
                {
                    b.Property<int>("WareId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<decimal?>("Converter")
                        .HasColumnType("decimal(10, 10)");

                    b.Property<DateTime?>("Date");

                    b.Property<string>("Mag_Nazwa");

                    b.Property<string>("Mag_Symbol");

                    b.Property<string>("Name");

                    b.Property<decimal?>("Quantity")
                        .HasColumnType("decimal(3, 2)");

                    b.Property<string>("TwG_Kod");

                    b.Property<string>("TwG_Nazwa");

                    b.Property<string>("Unit");

                    b.HasKey("WareId");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.ToTable("Wares");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SPMA.Models.Orders.Order", b =>
                {
                    b.HasOne("SPMA.Models.Sales.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("SPMA.Models.Orders.OrderBook", b =>
                {
                    b.HasOne("SPMA.Models.Production.Book", "Book")
                        .WithMany("OrderBooks")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Orders.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Production.ProductionState")
                        .WithMany("OrderBooks")
                        .HasForeignKey("ProductionStateId");
                });

            modelBuilder.Entity("SPMA.Models.Production.BookComponent", b =>
                {
                    b.HasOne("SPMA.Models.Production.Book", "Book")
                        .WithMany("BookComponents")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Production.Component", "Component")
                        .WithMany("BookComponents")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Warehouse.Ware", "Ware")
                        .WithMany()
                        .HasForeignKey("WareId");
                });

            modelBuilder.Entity("SPMA.Models.Production.Component", b =>
                {
                    b.HasOne("SPMA.Models.Warehouse.Ware", "Ware")
                        .WithMany()
                        .HasForeignKey("WareId");
                });

            modelBuilder.Entity("SPMA.Models.Production.InProduction", b =>
                {
                    b.HasOne("SPMA.Models.Production.Component", "Component")
                        .WithMany("InProduction")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Orders.OrderBook", "OrderBook")
                        .WithMany("InProduction")
                        .HasForeignKey("OrderBookId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Production.InProduction", "ParentReservation")
                        .WithMany()
                        .HasForeignKey("ParentReservationId");

                    b.HasOne("SPMA.Models.Production.ProductionSocket", "ProductionSocket")
                        .WithMany("InProduction")
                        .HasForeignKey("ProductionSocketId");

                    b.HasOne("SPMA.Models.Production.ProductionState", "ProductionState")
                        .WithMany("InProduction")
                        .HasForeignKey("ProductionStateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Warehouse.Ware", "Ware")
                        .WithMany()
                        .HasForeignKey("WareId");
                });

            modelBuilder.Entity("SPMA.Models.Production.InProductionRW", b =>
                {
                    b.HasOne("SPMA.Models.Production.InProduction", "InProduction")
                        .WithMany("InProductionRWs")
                        .HasForeignKey("InProductionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SPMA.Models.Warehouse.Ware", "Ware")
                        .WithMany()
                        .HasForeignKey("WareId");
                });
#pragma warning restore 612, 618
        }
    }
}
