
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomMonitor.Models
{
    public abstract class MonitorReading
    {
        public string Application { get; set; }

        public DateTime ReadingDtm { get; set; }
    }

    public class MonitorReadingSwitch : MonitorReading
    {
        public bool Value { get; set; }
    }

    public class MonitorReadingMeasure : MonitorReading
    {
        public decimal Value { get; set; }
    }
}