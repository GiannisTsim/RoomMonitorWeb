
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{
    public class SensorApplication
    {
        [Required]
        public string Application { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class ApplicationSwitch : SensorApplication
    {

        [Required]
        public string Value_0 { get; set; }

        [Required]
        public string Value_1 { get; set; }

    }

    public class ApplicationMeasure : SensorApplication
    {

        [Required]
        public string UnitMeasure { get; set; }

        public decimal? LimitMin { get; set; }
        public decimal? LimitMax { get; set; }
        public decimal? DefaultMin { get; set; }
        public decimal? DefaultMax { get; set; }

    }
}