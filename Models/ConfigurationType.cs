
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{
    public class ConfigurationType
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<string> MeasureApplications { get; set; }

        [Required]
        public List<string> SwitchApplications { get; set; }


    }

    public class ConfigurationTypeSensor
    {
        public string SensorType { get; set; }

        public string Application { get; set; }
    }
}