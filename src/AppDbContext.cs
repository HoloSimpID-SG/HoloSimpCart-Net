using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloSimpID
{
    public class AppDbContext : DbContext
    {
        public DbSet<Simp> dbSimp { get; set; }
        public DbSet<Cart> dbCart { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Simp>().ToTable("Simps");
            modelBuilder.Entity<Cart>().ToTable("Carts");

        }
    }
}
