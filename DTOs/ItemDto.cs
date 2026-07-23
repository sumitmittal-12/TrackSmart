namespace TrackSmart.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty ;

        public string CompanyName { get; set; } = string.Empty ;

        public string Description { get; set; } = string.Empty;

        public decimal OriginalPrice { get; set; }

        public decimal DiscountPercentage { get; set; }

        public int StockCount { get; set; }

        public int LowStockThreshold { get; set; }

        public int isActive { get; set; }

        public decimal FinalPrice => OriginalPrice * (1 - (DiscountPercentage / 100m));

        public bool IsLowStock => StockCount <= LowStockThreshold;

        public List<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();
    }
}
