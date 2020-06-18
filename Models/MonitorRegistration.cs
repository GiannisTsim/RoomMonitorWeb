
using System;
using System.ComponentModel.DataAnnotations;

namespace RoomMonitor.Models
{
    public class MonitorRegistration
    {
        [Required]
        public string MACAddress { get; set; }

        [Required]
        public string ConfigurationType { get; set; }

        // [Required]
        public string Manufacturer { get; set; }

        // [Required]
        public string Model { get; set; }

        // [Required]
        public string SWVersion { get; set; }

        // [Required]
        public DateTime SWUpdateDtm { get; set; }
    }
}