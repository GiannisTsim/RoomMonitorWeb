
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{

    public abstract class BaseHotelTagKey : BaseHotelKey
    {

        [StringLength(100)]
        public abstract string Tag { get; set; }
    }

    public class HotelTagKey : BaseHotelTagKey
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
        public override string Tag { get; set; }
    }
    public class HotelTag : BaseHotelTagKey
    {
        public override string HotelChain { get; set; }
        public override string CountryCode { get; set; }
        public override string Town { get; set; }
        public override string Suburb { get; set; }

        [Required]
        public override string Tag { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }

        public HotelTagKey GetKey() => new HotelTagKey
        {
            HotelChain = HotelChain,
            CountryCode = CountryCode,
            Town = Town,
            Suburb = Suburb,
            Tag = Tag
        };

        public void SetHotelKey(HotelKey hotelKey)
        {
            HotelChain = hotelKey.HotelChain;
            CountryCode = hotelKey.CountryCode;
            Town = hotelKey.Town;
            Suburb = hotelKey.Suburb;
        }
    }
}