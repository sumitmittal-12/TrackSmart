using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackSmart.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        // Holds the names of the items this supplier provides for the UI to display
        public List<ItemDto> SuppliedItems { get; set; } = new List<ItemDto>();
    }
}