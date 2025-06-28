using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(Environment.GetEnvironmentVariable("SQL_CONNECTION"))
                .LogTo(Console.WriteLine, LogLevel.Trace);//.EnableSensitiveDataLogging();
        }
        public static async Task EnsureMigrated()
        {
            using var context = new AppDbContext();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        #endregion
    }
}
