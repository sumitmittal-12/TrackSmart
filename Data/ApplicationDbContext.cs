using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TrackSmart.Models;

namespace TrackSmart.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ItemSupplier> ItemSuppliers { get; set; }

        // Add the custom rules for the decimals
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Item>()
                .Property(i => i.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Item>()
                .Property(i => i.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Entity<ItemSupplier>()
                .HasKey(x => new { x.ItemId, x.SupplierId });

            // EXPLICIT RELATIONSHIPS: Item -> ItemSupplier
            builder.Entity<ItemSupplier>()
                .HasOne(x => x.Item)
                .WithMany(i => i.ItemSuppliers)
                .HasForeignKey(x => x.ItemId);

            // EXPLICIT RELATIONSHIPS: Supplier -> ItemSupplier
            builder.Entity<ItemSupplier>()
                .HasOne(x => x.Supplier)
                .WithMany(s => s.ItemSuppliers)
                .HasForeignKey(x => x.SupplierId);
        }
    }
}
