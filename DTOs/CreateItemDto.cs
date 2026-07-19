using System.ComponentModel.DataAnnotations;

namespace TrackSmart.DTOs
{
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Please enter an item name.")]
        public string Name { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty; 

        [Required]
        [Range(0, 100000, ErrorMessage = "Price must be greater than zero.")]
        public decimal? OriginalPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be valid")]
        public decimal? DiscountPercentage { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "You must add at least 1 item to stock.")]
        public int? StockCount { get; set; }

        public int? LowStockThreshold { get; set; }

    }
}
