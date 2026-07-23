using System.ComponentModel.DataAnnotations;

namespace TrackSmart.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AddressLine { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        public string RetailerId { get; set; }

        public ApplicationUser? Retailer { get; set; }

        public bool isActive { get; set; } = true;

        public ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();
    }
}
