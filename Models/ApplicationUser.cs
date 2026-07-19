using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TrackSmart.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        [MaxLength(100)]
        public string? StoreName { get; set; }

        [MaxLength(100)]
        public string? AddressLine { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }
        public List<Item> InventoryItems { get; set; } = new List<Item>();
    }

}
