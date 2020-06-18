using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using RoomMonitor.Models;
using RoomMonitor.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class MonitorController : ControllerBase
    {
        private readonly MonitorStore _monitorStore;

        public MonitorController(MonitorStore monitorStore)
        {
            _monitorStore = monitorStore;
        }

        private HotelKey ParseHotelKeyFromClaims(ClaimsPrincipal principal)
        {
            string hotelClaim = principal.FindFirst(RoomMonitorConstants.ClaimTypes.Hotel).Value;
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            HotelKey hotelKey = JsonSerializer.Deserialize<HotelKey>(
                hotelClaim,
                serializeOptions);
            return hotelKey;
        }

        [Authorize(RoomMonitorConstants.Policies.DeviceAccess)]
        [HttpPut("api/monitors/registration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterMonitor([FromBody] MonitorRegistration monitorRegistration)
        {
            HotelKey hotelKey = ParseHotelKeyFromClaims(User);

            monitorRegistration.Manufacturer = "Espressif";
            monitorRegistration.Model = "ESP32";
            monitorRegistration.SWVersion = "0.0.1";
            monitorRegistration.SWUpdateDtm = DateTime.UtcNow;

            await _monitorStore.CreateAsync(hotelKey, monitorRegistration);

            return Ok(hotelKey);
        }


        // TODO: Better Error Handling and appropriate responses

        [Authorize(RoomMonitorConstants.Policies.DeviceAccess)]
        [HttpPost("api/monitors/records")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HandleMonitorRecord([FromBody] MonitorRecord monitorRecord)
        {
            HotelKey hotelKey = ParseHotelKeyFromClaims(User);

            MonitorKey monitorKey = new MonitorKey
            {
                HotelChain = hotelKey.HotelChain,
                CountryCode = hotelKey.CountryCode,
                Town = hotelKey.Town,
                Suburb = hotelKey.Suburb,
                MACAddress = monitorRecord.MACAddress
            };


            monitorRecord.Logs.ForEach(async log =>
            {
                try
                {
                    await _monitorStore.AddMonitorLogAsync(
                    hotelKey,
                    monitorRecord.MACAddress,
                    log);
                }
                catch (SqlException ex)
                {
                    StringBuilder errorMessages = new StringBuilder();
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                }
            });



            MonitorRoom monitorRoom = await _monitorStore.FindMonitorRoomDetailsAsync(monitorKey);
            if (monitorRoom != null)
            {
                monitorRecord.Readings.ForEach(async reading =>
                {
                    try
                    {
                        await _monitorStore.AddMonitorReadingAsync(
                            monitorRoom,
                            reading);
                    }
                    catch (SqlException ex)
                    {
                        StringBuilder errorMessages = new StringBuilder();
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine(errorMessages.ToString());
                    }
                });
            }

            return StatusCode(StatusCodes.Status201Created);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpGet("api/hotels/monitors/assigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MonitorRoom>>> GetMonitorRoomsByHotel([FromQuery] HotelKey hotelKey)
        {

            IEnumerable<MonitorRoom> result = await _monitorStore.FindAllMonitorRoomsByHotelAsync(hotelKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpGet("api/hotels/monitors/unassigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MonitorUnassigned>>> GetMonitorUnassignedByHotel([FromQuery] HotelKey hotelKey)
        {

            IEnumerable<MonitorUnassigned> result = await _monitorStore.FindAllMonitorUnassignedByHotelAsync(hotelKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpGet("api/hotels/monitors/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MonitorRoomAlternateKey>>> GetMonitorHistoryByHotel([FromQuery] HotelKey hotelKey)
        {
            IEnumerable<MonitorRoomAlternateKey> result = await _monitorStore.FindMonitorHistoryByHotelAsync(hotelKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpGet("api/hotels/rooms/monitors/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MonitorRoomAlternateKey>>> GetMonitorHistoryByRoom([FromQuery] RoomKey roomKey)
        {
            IEnumerable<MonitorRoomAlternateKey> result = await _monitorStore.FindMonitorHistoryByRoomAsync(roomKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpPut("api/hotels/monitors/assigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MonitorRoom>> AssignMonitor([FromQuery] MonitorKey monitorKey, [FromBody] MonitorRoom monitorRoom)
        {
            monitorRoom.SetMonitorKey(monitorKey);
            await _monitorStore.AssignMonitorAsync(monitorRoom);
            MonitorRoom result = await _monitorStore.FindMonitorRoomDetailsAsync(monitorKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.HotelAdminAccess)]
        [HttpPut("api/hotels/monitors/unassigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MonitorRoom>> DeassignMonitor([FromQuery] MonitorKey monitorKey)
        {
            await _monitorStore.DeassignMonitorAsync(monitorKey);
            var result = await _monitorStore.FindMonitorUnassignedDetailsAsync(monitorKey);
            return Ok(result);
        }
    }
}