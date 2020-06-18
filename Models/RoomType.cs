
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{
    public class RoomType
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

    }
}