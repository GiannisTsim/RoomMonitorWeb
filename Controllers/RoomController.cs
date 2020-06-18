using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using IdentityServer4;

using RoomMonitor.Models;
using RoomMonitor.Data;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {
        private readonly RoomStore _roomStore;

        public RoomController(RoomStore roomStore)
        {
            _roomStore = roomStore;
        }


        [Authorize(RoomMonitorConstants.Policies.GenericUserAccess)]
        [HttpGet("/api/hotels/rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsByHotel([FromQuery] HotelKey hotelKey)
        {
            IEnumerable<Room> result = await _roomStore.FindAllByHotelAsync(hotelKey);
            return Ok(result);
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("/api/room-types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomTypes()
        {
            IEnumerable<RoomType> result = await _roomStore.FindAllRoomTypesAsync();
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.GenericUserAccess)]
        [HttpGet("api/hotels/rooms/detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Room>> GetRoomDetails([FromQuery] RoomKey roomKey)
        {
            Room result = await _roomStore.FindDetailsAsync(roomKey);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpPost("/api/hotels/rooms")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Room>> AddRoom([FromQuery] HotelKey hotelKey, [FromBody]Room room)
        {
            room.SetHotelKey(hotelKey);
            if (await _roomStore.CheckExistanceAsync(room.GetKey()))
            {
                return ValidationProblem(
                    title: "Resource already exists",
                    detail: string.Format(
                        "{0} '{1}' at {2},{3},{4},{5} already exists",
                        room.RoomType,
                        room.Name,
                        room.HotelChain,
                        room.CountryCode,
                        room.Town,
                        room.Suburb)
                    );
            }
            await _roomStore.CreateAsync(room);

            return CreatedAtAction(
                nameof(GetRoomDetails),
                room.GetKey(),
                room);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpPut("/api/hotels/rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Room>> EditRoom([FromQuery] RoomKey roomKey, [FromBody] Room room)
        {
            room.SetHotelKey(roomKey);
            if (!await _roomStore.CheckExistanceAsync(roomKey))
            {
                return NotFound();
            }
            await _roomStore.UpdateAsync(roomKey, room);
            return Ok(room);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpDelete("/api/hotels/rooms")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom([FromQuery] RoomKey roomKey)
        {
            if (!await _roomStore.CheckExistanceAsync(roomKey))
            {
                return NotFound();
            }
            await _roomStore.DeleteAsync(roomKey);
            return NoContent();
        }

    }
}