
using System.ComponentModel.DataAnnotations;

namespace RoomMonitor.Models
{
    public class UserInvitation
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
