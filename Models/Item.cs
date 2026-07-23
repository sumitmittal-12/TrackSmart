using System.ComponentModel.DataAnnotations;

namespace TrackSmart.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100000)]
        public decimal OriginalPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        public int StockCount { get; set; }

        public int LowStockThreshold { get; set; }

        public DateTime DateOfPurchase { get; set; } = DateTime.UtcNow;

        public bool isActive { get; set; } = true;

        // --- Calculated Properties ---
        // Entity Framework ignores getter-only properties by default, 
        // so these will NOT create columns in the database. They compute in real-time.


        public decimal FinalPrice => OriginalPrice * (1 - (DiscountPercentage / 100m));

        public bool IsLowStock => StockCount <= LowStockThreshold;

        public string? RetailerId { get; set; }

        public ApplicationUser? Retailer { get; set; }

        public ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();
    }
}

