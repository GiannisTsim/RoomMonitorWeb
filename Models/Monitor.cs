
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{
    public abstract class BaseMonitorKey : BaseHotelKey
    {
        [StringLength(17)]
        public abstract string MACAddress { get; set; }
    }

    public class MonitorRoomAlternateKey : HotelKey
    {
        [StringLength(100)]
        [BindRequired]
        public string RoomType { get; set; }

        [StringLength(100)]
        [BindRequired]
        public string Room { get; set; }

        [StringLength(100)]
        [BindRequired]
        public string Monitor { get; set; }
    }


    public class MonitorKey : BaseMonitorKey
    {

        [BindRequired]
        public override string HotelChain { get; set; }

        [BindRequired]
        public override string CountryCode { get; set; }

        [BindRequired]
        public override string Town { get; set; }

        [BindRequired]
        public override string Suburb { get; set; }

        [BindRequired]
        public override string MACAddress { get; set; }
    }


    public abstract class Monitor : BaseMonitorKey
    {
        public string MonitorType { get; set; }
        public string ConfigurationType { get; set; }

        [JsonPropertyName("SWVersion")]
        public string SWVersion { get; set; }

        [JsonPropertyName("SWUpdateDtm")]
        public DateTime? SWUpdateDtm { get; set; }
        public DateTime? RegistrationDtm { get; set; }
        public string RegistrationInfo { get; set; }
    }


    public class MonitorRoom : Monitor
    {
        public override string HotelChain { get; set; }
        public override string CountryCode { get; set; }
        public override string Town { get; set; }
        public override string Suburb { get; set; }

        [JsonPropertyName("MACAddress")]
        public override string MACAddress { get; set; }

        [Required]
        public string RoomType { get; set; }

        [Required]
        public string Room { get; set; }

        [Required]
        public string Monitor { get; set; }

        public DateTime? PlacementDtm { get; set; }

        public MonitorKey GetKey() => new MonitorKey
        {
            HotelChain = HotelChain,
            CountryCode = CountryCode,
            Town = Town,
            Suburb = Suburb,
            MACAddress = MACAddress
        };

        public void SetMonitorKey(MonitorKey monitorKey)
        {
            HotelChain = monitorKey.HotelChain;
            CountryCode = monitorKey.CountryCode;
            Town = monitorKey.Town;
            Suburb = monitorKey.Suburb;
            MACAddress = monitorKey.MACAddress;
        }
    }

    public class MonitorUnassigned : Monitor
    {
        public override string HotelChain { get; set; }
        public override string CountryCode { get; set; }
        public override string Town { get; set; }
        public override string Suburb { get; set; }

        [JsonPropertyName("MACAddress")]
        public override string MACAddress { get; set; }

        public MonitorKey GetKey() => new MonitorKey
        {
            HotelChain = HotelChain,
            CountryCode = CountryCode,
            Town = Town,
            Suburb = Suburb,
            MACAddress = MACAddress
        };
    }
}