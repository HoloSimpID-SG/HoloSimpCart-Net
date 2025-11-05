using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Diagnostics;

namespace HoloSimpID {
  public class AppDbContext : DbContext {
#region Mappings
    public DbSet<CommandVCS> CommandVCS { get; set; } = null!;

    public DbSet<Simp> Simps { get; set; }          = null!;
    public DbSet<Cart> Carts { get; set; }          = null!;
    public DbSet<CartItems> CartItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<CommandVCS>(entity => {
        entity.HasKey(e => e.command_name);
        entity.Property(e => e.version_hash).IsRequired();
        entity.Property(e => e.last_update).HasDefaultValueSql("CURRENT_TIMESTAMP");
      });

      modelBuilder.Entity<CartItems>(entity => {
        entity.HasKey(e => new { e.cartDex, e.simpDex });
        entity.Property(e => e.Items).HasColumnType("jsonb");
        entity.Property(e => e.Quantities).HasColumnType("integer[]");
      });

      modelBuilder.Entity<Cart>(entity => {
        entity.Property(e => e.ShippingCost).HasColumnType("numeric");
        entity.Property(e => e.DateOpen).HasColumnType("timestamptz");
        entity.Property(e => e.DatePlan).HasColumnType("timestamptz");
        entity.Property(e => e.DateClose).HasColumnType("timestamptz");
        entity.Property(e => e.DateDelivered).HasColumnType("timestamptz");
      }
      );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      if (optionsBuilder.IsConfigured)
        return;

      var dataSourceBuilder =
          new NpgsqlDataSourceBuilder(Environment.GetEnvironmentVariable("SQL_CONNECTION"));
      dataSourceBuilder.EnableDynamicJson();
      NpgsqlDataSource dataSource = dataSourceBuilder.Build();

      optionsBuilder.UseNpgsql(dataSource).LogTo(Console.WriteLine, LogLevel.Trace);
    }

    public static async Task EnsureMigrated() {
      var dataSourceBuilder =
          new NpgsqlDataSourceBuilder(Environment.GetEnvironmentVariable("SQL_CONNECTION"));
      dataSourceBuilder.EnableDynamicJson();
      NpgsqlDataSource dataSource = dataSourceBuilder.Build();

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
      optionsBuilder.UseNpgsql(dataSource);

      var db = new AppDbContext(optionsBuilder.Options);
      try {
        await db.Database.MigrateAsync();
        // Warm-Up
        _ = await db.CommandVCS.FirstOrDefaultAsync();

        _ = await db.Carts.FirstOrDefaultAsync();
        _ = await db.Simps.FirstOrDefaultAsync();
        _ = await db.CartItems.FirstOrDefaultAsync();
        // Finishing
      } catch (Exception ex) {
        Console.WriteLine("Error during Migration:");
        Console.WriteLine(ex.ToStringDemystified());
      } finally {
        await db.DisposeAsync();
      }
    }

    public AppDbContext() {}
    public AppDbContext(DbContextOptions options) : base(options) {}
#endregion
  }
  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext> {
    public AppDbContext CreateDbContext(string[] args) {
      var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION") ??
                             "Host=localhost;Database=test;Username=test;Password=test";

      var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
      dataSourceBuilder.EnableDynamicJson();
      var dataSource = dataSourceBuilder.Build();

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
      optionsBuilder.UseNpgsql(dataSource);

      return new AppDbContext(optionsBuilder.Options);
    }
  }
}
