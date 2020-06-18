using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using RoomMonitor.Models;
using RoomMonitor.Data;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class SensorApplicationController : ControllerBase
    {
        private readonly SensorApplicationStore _sensorApplicationStore;
        public SensorApplicationController(SensorApplicationStore sensorApplicationStore)
        {
            _sensorApplicationStore = sensorApplicationStore;
        }


        [HttpGet("api/applications/measure")]
        public async Task<ActionResult<IEnumerable<ApplicationMeasure>>> GetMeasureApplications()
        {
            IEnumerable<ApplicationMeasure> result = await _sensorApplicationStore.FindAllApplicationMeasureAsync();
            return Ok(result);
        }


        [HttpGet("api/applications/switch")]
        public async Task<ActionResult<IEnumerable<ApplicationSwitch>>> GetSwitchApplications()
        {
            IEnumerable<ApplicationSwitch> result = await _sensorApplicationStore.FindAllApplicationSwitchAsync();
            return Ok(result);
        }


        [HttpPost("api/applications/measure")]
        public async Task<ActionResult<ApplicationMeasure>> AddApplicationMeasure([FromBody] ApplicationMeasure applicationMeasure)
        {
            await _sensorApplicationStore.CreateApplicationMeasureAsync(applicationMeasure);
            // return CreatedAtAction(
            //     nameof(GetSensorApplicationDetails),
            //     sensorApplication.GetKey(),
            //     sensorApplication);
            return StatusCode(201);
        }


        [HttpPost("api/applications/switch")]
        public async Task<ActionResult<ApplicationSwitch>> AddApplicationSwitch([FromBody] ApplicationSwitch applicationSwitch)
        {
            await _sensorApplicationStore.CreateApplicationSwitchAsync(applicationSwitch);
            // return CreatedAtAction(
            //     nameof(GetSensorApplicationDetails),
            //     sensorApplication.GetKey(),
            //     sensorApplication);
            return StatusCode(201);
        }

    }
}