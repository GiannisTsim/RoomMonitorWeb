
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoomMonitor.Models
{

    // interface IBaseHotelKey {
    //     string HotelChain { get; set; }
    //     string CountryCode { get; set; }]
    //     string Town { get; set; }
    //     string Suburb { get; set; }
    // }
    public abstract class BaseHotelKey
    {

        [StringLength(100)]
        public abstract string HotelChain { get; set; }

        [StringLength(2)]
        public abstract string CountryCode { get; set; }

        [StringLength(100)]
        public abstract string Town { get; set; }

        [StringLength(100)]
        public abstract string Suburb { get; set; }
    }

    public class HotelKey : BaseHotelKey
    {
        [BindRequired]
        public override string HotelChain { get; set; }

        [BindRequired]
        public override string CountryCode { get; set; }

        [BindRequired]
        public override string Town { get; set; }

        [BindRequired]
        public override string Suburb { get; set; }
    }

    public class Hotel : BaseHotelKey
    {
        [Required]
        public override string HotelChain { get; set; }

        [Required]
        public override string CountryCode { get; set; }

        [Required]
        public override string Town { get; set; }

        [Required]
        public override string Suburb { get; set; }

        [Required]
        [Range(0, 5)]
        public int? NumStar { get; set; }

        public HotelKey GetKey() => new HotelKey
        {
            HotelChain = this.HotelChain,
            CountryCode = this.CountryCode,
            Town = this.Town,
            Suburb = this.Suburb
        };
    }
}