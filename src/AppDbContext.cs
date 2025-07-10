using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace HoloSimpID
{
    public class AppDbContext : DbContext
    {
        #region Mappings
        public DbSet<Simp> Simps { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItems> CartItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItems>()
                .HasKey(ci => new { ci.cartDex, ci.simpDex });

            modelBuilder.Entity<CartItems>()
                .Property(ci => ci.Items)
                .HasColumnType("jsonb");

            modelBuilder.Entity<CartItems>()
                .Property(ci => ci.Quantities)
                .HasColumnType("integer[]");

            modelBuilder.Entity<Cart>()
                .Property(c => c.ShippingCost)
                .HasColumnType("numeric");

            modelBuilder.Entity<Cart>()
                .Property(c => c.DateOpen)
                .HasColumnType("timestamptz");
            modelBuilder.Entity<Cart>()
                .Property(c => c.DatePlan)
                .HasColumnType("timestamptz");
            modelBuilder.Entity<Cart>()
                .Property(c => c.DateClose)
                .HasColumnType("timestamptz");
            modelBuilder.Entity<Cart>()
                .Property(c => c.DateDelivered)
                .HasColumnType("timestamptz");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(
                Environment.GetEnvironmentVariable("SQL_CONNECTION"));
            dataSourceBuilder.EnableDynamicJson();
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();

            optionsBuilder
                .UseNpgsql(dataSource)
                .LogTo(Console.WriteLine, LogLevel.Trace);
        }

        public static async Task EnsureMigrated()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(
                Environment.GetEnvironmentVariable("SQL_CONNECTION"));
            dataSourceBuilder.EnableDynamicJson();
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(dataSource);

            var db = new AppDbContext(optionsBuilder.Options);
            await db.Database.MigrateAsync();
            // Warm-Up
            _ = await db.Carts.FirstOrDefaultAsync();
            _ = await db.Simps.FirstOrDefaultAsync();
            _ = await db.CartItems.FirstOrDefaultAsync();
            // Finishing
            await db.DisposeAsync();
        }

        public AppDbContext() { }
        public AppDbContext(DbContextOptions options) : base(options) { }
        #endregion
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION")
                                   ?? "Host=localhost;Database=test;Username=test;Password=test";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(dataSource);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}