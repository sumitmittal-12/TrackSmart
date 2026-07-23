using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TrackSmart.DTOs
{
    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Company Name is required.")]
        [MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        public string ContactPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AddressLine { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(50)]
        public string? PostalCode { get; set; }

        // THE KEY ADDITION: Holds the IDs of the items this supplier provides
        public List<int> ItemIds { get; set; } = new List<int>();
    }
}