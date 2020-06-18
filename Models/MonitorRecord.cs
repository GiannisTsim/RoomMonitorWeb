
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RoomMonitor.Models
{
    public class MonitorRecord
    {

        [Required]
        public string MACAddress { get; set; }

        [Required]
        public string ConfigurationType { get; set; }

        [Required]
        public List<MonitorLog> Logs { get; set; }

        [Required]
        public List<MonitorReading> Readings { get; set; }
    }
}