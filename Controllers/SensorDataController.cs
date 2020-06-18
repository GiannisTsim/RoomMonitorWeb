using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using RoomMonitor.Models;
using RoomMonitor.Data;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class SensorDataController : ControllerBase
    {
        private readonly SensorDataStore _sensorDataStore;

        public SensorDataController(SensorDataStore sensorDataStore)
        {
            _sensorDataStore = sensorDataStore;
        }

        [Authorize(RoomMonitorConstants.Policies.HotelUserAccess)]
        [HttpGet("api/readings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<HotelData>>> GetReadings([FromQuery] MonitorRoomAlternateKey monitorRoomAltKey)
        {
            var result = await _sensorDataStore.FindReadingsAsync(monitorRoomAltKey);
            return Ok(result);
        }

        [Authorize(RoomMonitorConstants.Policies.HotelUserAccess)]
        [HttpGet("api/hotels/readings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelData>> GetHotelReadings([FromQuery] HotelKey hotelKey)
        {
            var result = await _sensorDataStore.FindReadingsByHotelAsync(hotelKey);
            return Ok(result);
        }

        [Authorize(RoomMonitorConstants.Policies.HotelUserAccess)]
        [HttpGet("api/hotels/rooms/readings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomData>> GetRoomReadings([FromQuery] RoomKey roomKey)
        {
            var result = await _sensorDataStore.FindReadingsByRoomAsync(roomKey);
            return Ok(result);
        }

        [Authorize(RoomMonitorConstants.Policies.HotelUserAccess)]
        [HttpGet("api/hotels/rooms/monitors/readings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MonitorData>> GetMonitorReadings([FromQuery] MonitorRoomAlternateKey monitorRoomAltKey)
        {
            var result = await _sensorDataStore.FindReadingsByMonitorAsync(monitorRoomAltKey);
            return Ok(result);
        }
    }
}