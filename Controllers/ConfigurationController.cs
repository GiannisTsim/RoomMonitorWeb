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
    public class ConfigurationController : ControllerBase
    {
        private readonly ConfigurationStore _configurationStore;
        public ConfigurationController(ConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }


        [HttpGet("api/configuration-types")]
        public async Task<ActionResult<IEnumerable<ConfigurationType>>> GetConfigurationTypes()
        {
            IEnumerable<ConfigurationType> result = await _configurationStore.FindAllAsync();
            return Ok(result);
        }


        // [HttpPost("api/configuration-types")]
        // public async Task<ActionResult<ConfigurationType>> AddConfigurationType()
        // {
        //     return new List<string> { };
        // }

    }
}