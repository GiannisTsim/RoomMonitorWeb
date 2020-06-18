
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomMonitor.Models
{
    public class MonitorLog
    {

        [Required]
        public DateTime LogDtm { get; set; }

        [Required]
        public decimal BatteryLevel { get; set; }

        [Required]
        public int ConnFail { get; set; }

        [Required]
        public int PostFail { get; set; }

        [Required]
        public int Rssi { get; set; }
    }
}