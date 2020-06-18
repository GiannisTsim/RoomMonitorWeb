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
    public class HotelController : ControllerBase
    {
        private readonly HotelStore _hotelStore;
        public HotelController(HotelStore hotelStore)
        {
            _hotelStore = hotelStore;
        }


        [Authorize(RoomMonitorConstants.Policies.SystemAdminAccess)]
        [HttpGet("api/hotels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHotels()
        {
            IEnumerable<Hotel> result = await _hotelStore.FindAllAsync();
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.GenericUserAccess)]
        [HttpGet("api/hotels/detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Hotel>> GetHotelDetails([FromQuery] HotelKey hotelKey)
        {
            Hotel result = await _hotelStore.FindDetailsAsync(hotelKey);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }


        [Authorize(RoomMonitorConstants.Policies.SystemAdminAccess)]
        [HttpPost("api/hotels")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Hotel>> AddHotel([FromBody] Hotel hotel)
        {
            if (await _hotelStore.CheckExistanceAsync(hotel.GetKey()))
            {
                return ValidationProblem(
                    title: "Resource already exists",
                    detail: string.Format(
                        "{0} at {1},{2},{3} already exists",
                        hotel.HotelChain,
                        hotel.CountryCode,
                        hotel.Town,
                        hotel.Suburb)
                );
            }
            await _hotelStore.CreateAsync(hotel);
            return CreatedAtAction(
                nameof(GetHotelDetails),
                hotel.GetKey(),
                hotel);
        }


        [Authorize(RoomMonitorConstants.Policies.SystemAdminAccess)]
        [HttpPut("api/hotels/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Hotel>> EditHotel([FromQuery] HotelKey hotelKey, [FromBody] Hotel hotel)
        {
            if (!await _hotelStore.CheckExistanceAsync(hotelKey))
            {
                return NotFound();
            }
            await _hotelStore.UpdateAsync(hotelKey, hotel);
            return Ok(hotel);
        }


        [Authorize(RoomMonitorConstants.Policies.SystemAdminAccess)]
        [HttpDelete("api/hotels/")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel([FromQuery] HotelKey hotelKey)
        {
            if (!await _hotelStore.CheckExistanceAsync(hotelKey))
            {
                return NotFound();
            }
            await _hotelStore.DeleteAsync(hotelKey);
            return NoContent();
        }
    }
}