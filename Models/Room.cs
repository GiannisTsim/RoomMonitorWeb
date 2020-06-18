
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{

    public abstract class BaseRoomKey : BaseHotelKey
    {

        // public override string HotelChain { get; set; }
        // public override string CountryCode { get; set; }
        // public override string Town { get; set; }
        // public override string Suburb { get; set; }

        [StringLength(100)]
        public abstract string RoomType { get; set; }

        [StringLength(100)]
        public abstract string Name { get; set; }
    }

    public class RoomKey : BaseRoomKey
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
        public override string RoomType { get; set; }

        [BindRequired]
        public override string Name { get; set; }

    }

    public class Room : BaseRoomKey
    {
        public override string HotelChain { get; set; }
        public override string CountryCode { get; set; }
        public override string Town { get; set; }
        public override string Suburb { get; set; }

        [Required]
        public override string RoomType { get; set; }

        [Required]
        public override string Name { get; set; }

        [Required]
        public List<string> Tags { get; set; }

        public RoomKey GetKey() => new RoomKey
        {
            HotelChain = HotelChain,
            CountryCode = CountryCode,
            Town = Town,
            Suburb = Suburb,
            RoomType = RoomType,
            Name = Name
        };

        public void SetHotelKey(HotelKey hotelKey)
        {
            HotelChain = hotelKey.HotelChain;
            CountryCode = hotelKey.CountryCode;
            Town = hotelKey.Town;
            Suburb = hotelKey.Suburb;
        }

        public void SetHotelKey(RoomKey roomKey)
        {
            HotelChain = roomKey.HotelChain;
            CountryCode = roomKey.CountryCode;
            Town = roomKey.Town;
            Suburb = roomKey.Suburb;
        }
    }
}