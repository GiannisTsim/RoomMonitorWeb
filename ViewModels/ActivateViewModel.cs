using System.ComponentModel.DataAnnotations;

namespace RoomMonitor.ViewModels
{
    public class ActivateViewModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}