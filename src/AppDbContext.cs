using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Npgsql;

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

        public override int SaveChanges()
        {
            DateConvert();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            DateConvert();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void DateConvert()
        {
            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                foreach (PropertyEntry prop in entry.Properties)
                {
                    if (prop.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Local)
                    {
                        prop.CurrentValue = dt.ToUniversalTime();
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (optionsBuilder.IsConfigured) return;
            //
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(Environment.GetEnvironmentVariable("SQL_CONNECTION"));
            dataSourceBuilder.EnableDynamicJson();

            optionsBuilder
                .UseNpgsql(dataSourceBuilder.Build())
                .LogTo(Console.WriteLine, LogLevel.Trace);
        }

        public static async Task EnsureMigrated()
        {
            await using var context = new AppDbContext();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        #endregion
    }
}