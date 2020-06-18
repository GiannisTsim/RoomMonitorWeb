using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using RoomMonitor.Models;
using RoomMonitor.Data;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class HotelTagController : ControllerBase
    {
        private readonly HotelTagStore _hotelTagStore;

        public HotelTagController(HotelTagStore hotelTagStore)
        {
            _hotelTagStore = hotelTagStore;
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpGet("/api/hotels/tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelTag>>> GetTagsByHotel([FromQuery] HotelKey hotelKey)
        {
            IEnumerable<HotelTag> result = await _hotelTagStore.FindAllByHotelAsync(hotelKey);
            return Ok(result);
        }

        [Authorize(RoomMonitorConstants.Policies.GenericUserAccess)]
        [HttpGet("api/hotels/tags/detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelTag>> GetHotelTagDetails([FromQuery] HotelTagKey hotelTagKey)
        {
            HotelTag result = await _hotelTagStore.FindDetailsAsync(hotelTagKey);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpPost("/api/hotels/tags")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<HotelTag>>> AddHotelTag(
            [FromQuery] HotelKey hotelKey,
            [FromBody]HotelTag hotelTag)
        {
            hotelTag.SetHotelKey(hotelKey);
            if (await _hotelTagStore.CheckExistanceAsync(hotelTag.GetKey()))
            {
                return ValidationProblem(
                    title: "Resource already exists",
                    detail: string.Format(
                        "{0} at {1},{2},{3},{4} already exists",
                        hotelTag.Tag,
                        hotelTag.HotelChain,
                        hotelTag.CountryCode,
                        hotelTag.Town,
                        hotelTag.Suburb)
                    );
            }
            await _hotelTagStore.CreateAsync(hotelTag);
            return CreatedAtAction(
                nameof(GetHotelTagDetails),
                hotelTag.GetKey(),
                hotelTag);
        }


        // [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        // [HttpPut("/api/hotels/{hotelId}/locations/{hotelLocationId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<ActionResult<HotelLocation>> EditHotelLocation(int hotelId, int hotelLocationId, HotelLocation hotelLocation)
        // {
        //     if (!await _roomStore.CheckExistanceAsync(hotelId, hotelLocationId))
        //     {
        //         return NotFound();
        //     }
        //     HotelLocation result = await _roomStore.UpdateAsync(hotelLocationId, hotelLocation);
        //     return Ok(result);
        // }


        // [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        // [HttpDelete("/api/hotels/{hotelId}/locations/{hotelLocationId}")]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> DeleteHotelLocation(int hotelId, int hotelLocationId)
        // {
        //     if (!await _roomStore.CheckExistanceAsync(hotelId, hotelLocationId))
        //     {
        //         return NotFound();
        //     }
        //     await _roomStore.DeleteAsync(hotelLocationId);
        //     return NoContent();
        // }

    }
}