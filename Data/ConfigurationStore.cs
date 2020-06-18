using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;

using RoomMonitor.Models;

namespace RoomMonitor.Data
{
    public class ConfigurationStore
    {
        private readonly string _connectionString;
        public ConfigurationStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ConfigurationType>> FindAllAsync()
        {
            var lookup = new Dictionary<string, ConfigurationType>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<ConfigurationType, ConfigurationTypeSensor, ConfigurationType>(
                @"  SELECT  ConfigurationType.ConfigurationType AS Name, 
                            ConfigurationType.[Description], 
                            'Switch' AS SensorType, 
                            ApplicationSwitch AS Application
                    FROM ConfigurationType
                        INNER JOIN ConfigurationSwitch
                        ON ConfigurationType.ConfigurationType = ConfigurationSwitch.ConfigurationType
                    UNION
                    SELECT  ConfigurationType.ConfigurationType AS Name,
                            ConfigurationType.[Description], 
                            'Measure' AS SensorType, 
                            ApplicationMeasure AS Application
                    FROM ConfigurationType
                        INNER JOIN ConfigurationMeasure
                        ON ConfigurationType.ConfigurationType = ConfigurationMeasure.ConfigurationType",
                (c, s) =>
                {
                    string key = c.Name;
                    if (!lookup.TryGetValue(key, out ConfigurationType configurationType))
                    {
                        lookup.Add(key, configurationType = c);
                        configurationType.MeasureApplications = new List<string>();
                        configurationType.SwitchApplications = new List<string>();
                    }
                    switch (s.SensorType)
                    {
                        case "Measure":
                            configurationType.MeasureApplications.Add(s.Application);
                            break;
                        case "Switch":
                            configurationType.SwitchApplications.Add(s.Application);
                            break;
                    }
                    return configurationType;
                },
                splitOn: "SensorType"
            );
            return lookup.Values;
        }
    }
}