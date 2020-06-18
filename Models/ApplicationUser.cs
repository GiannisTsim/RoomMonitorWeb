
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoomMonitor.Models
{
    public class ApplicationUser
    {
        [JsonPropertyName("userId")]
        public int PersonId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonIgnore]
        public string NormalizedEmail { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public string SecurityStamp { get; set; }

        public string Role { get; set; }

        public string HotelChain { get; set; }

        public string CountryCode { get; set; }

        public string Town { get; set; }

        public string Suburb { get; set; }
    }
}
