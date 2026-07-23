namespace TrackSmart.Models
{
    public class ItemSupplier
    {
        // Foreign Key to Item
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;

        // Foreign Key to Supplier
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
    }
}
