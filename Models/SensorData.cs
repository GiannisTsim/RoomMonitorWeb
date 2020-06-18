
using System;
using System.Collections.Generic;

namespace RoomMonitor.Models
{
    // public abstract class SensorData<T>
    // {
    //     public string Application { get; set; }

    //     public abstract string SensorType { get; }

    //     public List<T> Data { get; set; }
    // }

    // public class SensorMeasureData : SensorData<ReadingMeasure>
    // {
    //     public override string SensorType { get => "Measure"; }
    // }

    // public class SensorSwitchData : SensorData<ReadingSwitch>
    // {
    //     public override string SensorType { get => "Switch"; }
    // }

    // public abstract class Reading<T>
    // {
    //     public string HotelChain { get; set; }

    //     public string CountryCode { get; set; }

    //     public string Town { get; set; }

    //     public string Suburb { get; set; }

    //     public string RoomType { get; set; }

    //     public string Room { get; set; }

    //     public string Monitor { get; set; }

    //     public DateTime ReadingDtm { get; set; }

    //     public abstract T Value { get; set; }
    // }

    // public class ReadingMeasure : Reading<decimal>
    // {
    //     public override decimal Value { get; set; }
    // }

    // public class ReadingSwitch : Reading<bool>
    // {
    //     public override bool Value { get; set; }
    // }


    // ########################### Sensor Data ###########################
    public abstract class SensorData
    {
        public DateTime ReadingDtm { get; set; }
    }

    public class SensorMeasureData : SensorData
    {
        public decimal Value { get; set; }
    }

    public class SensorSwitchData : SensorData
    {
        public decimal Value { get; set; }
    }


    // ########################### Application Data ###########################
    public abstract class ApplicationData
    {
        public string Application { get; set; }
        public abstract string SensorType { get; }
    }

    public class ApplicationMeasureData : ApplicationData
    {
        public override string SensorType { get => "Measure"; }
        public List<SensorMeasureData> SensorData { get; set; }
    }

    public class ApplicationSwitchData : ApplicationData
    {
        public override string SensorType { get => "Switch"; }
        public List<SensorSwitchData> SensorData { get; set; }
    }

    // ########################### Monitor Data ###########################
    public class MonitorData
    {
        public string Monitor { get; set; }
        public List<ApplicationMeasureData> ApplicationMeasureData { get; set; }
        public List<ApplicationSwitchData> ApplicationSwitchData { get; set; }
    }

    // ########################### Room Data ###########################
    public class RoomData
    {
        public string RoomType { get; set; }
        public string Room { get; set; }
        public List<MonitorData> MonitorData { get; set; }
    }

    // ########################### Hotel Data ###########################
    public class HotelData
    {
        public string HotelChain { get; set; }
        public string CountryCode { get; set; }
        public string Town { get; set; }
        public string Suburb { get; set; }
        public List<RoomData> RoomData { get; set; }
    }
}
