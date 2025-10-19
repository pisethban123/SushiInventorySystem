using SushiInventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace SushiInventorySystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stock – Branch Relationships
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.From)
                .WithMany(b => b.TransfersFrom)
                .HasForeignKey(t => t.FromBranch)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.To)
                .WithMany(b => b.TransfersTo)
                .HasForeignKey(t => t.ToBranch)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
