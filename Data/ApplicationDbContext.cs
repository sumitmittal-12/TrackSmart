using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrackSmart.Models;

namespace TrackSmart.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Item> Items { get; set; }

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
        }
    }
}
