using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPMA.Models.Core;
using SPMA.Models.Optima;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Sales;
using SPMA.Models.Stocktaking;
using SPMA.Models.Test;
using SPMA.Models.Warehouse;
using System;

namespace SPMA.Data
{
    /// <summary>
    /// DbContext used for reading and modifying Application Database
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        #region Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        #endregion

        #region Override methods
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<InProduction>().Ignore(ip => ip.OrderBook );
            builder.Entity<BookComponent>()
                .HasOne(x => x.Book)
                .WithMany(x => x.BookComponents)
                .HasForeignKey(x => x.BookId);
            builder.Entity<BookComponent>().Ignore(bc => bc.InProduction);
            builder.Entity<BookComponent>()
                .HasOne(x => x.Component)
                .WithMany(x => x.BookComponents)
                .HasForeignKey(x => x.ComponentId);
            builder.Entity<ProductionState>()
                .HasData(new ProductionState[] 
                { new ProductionState { Name =  "Bezczynny", Comment = "Oczekujący na dodanie do produkcji",  ProductionStateCode = 1, ProductionStateId = 1},
                  new ProductionState { Name =  "Oczekuje", Comment = "Oczekujący w kolejce do produkcji",  ProductionStateCode = 2, ProductionStateId = 2},
                  new ProductionState { Name =  "Kupna", Comment = "Część kupna nie uwzględniana w produkcji",  ProductionStateCode = 100, ProductionStateId = 3}
                });

            builder.Entity<ProductionState>().HasIndex(i => i.ProductionStateCode).IsUnique();

            builder.Entity<Ware>().HasIndex(u => u.Code).IsUnique();
            builder.Entity<Service>().HasIndex(s => s.Name).IsUnique();

            //builder.Entity<Component>()
            //    .HasOne(c => c.Ware)
            //    .WithMany(e => e.Components);
            builder.Entity<Component>().HasIndex(u => u.Number).IsUnique();

            builder.Entity<Book>().HasIndex(u => u.OfficeNumber).IsUnique();
            //builder.Entity<Book>().Ignore(b => b.BookComponents);
            builder.Entity<Book>();
            builder.Entity<Order>().HasIndex(o => o.Number).IsUnique();
            builder.Entity<Order>().Ignore(o => o.OrderBooks);


            base.OnModelCreating(builder);
        }
        #endregion

        #region DbSets
        public DbSet<InProductionRW> InProductionRWs { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Ware> Wares { get; set; }
        public DbSet<InProduction> InProduction { get; set; }
        public DbSet<ProductionSocket> ProductionSockets { get; set; }
        public DbSet<ProductionState> ProductionStates { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<BookComponent> BookComponents { get; set; }
        public DbSet<OrderBook> OrderBooks { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { set; get; }
        public DbSet<ReservedItem> ReservedItems { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        #endregion
    }
}
