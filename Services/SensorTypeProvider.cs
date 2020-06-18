using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RoomMonitor.Services
{
    public class SensorTypeProvider
    {
        private readonly Dictionary<string, string> applicationSensorTypeLookup;
        private readonly string _connectionString;

        public SensorTypeProvider(IConfiguration configuration)
        {
            applicationSensorTypeLookup = new Dictionary<string, string>();
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            RefreshLookup();
        }

        private void RefreshLookup()
        {
            applicationSensorTypeLookup.Clear();
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            var result = connection.Query<dynamic>(
                "SELECT Application, SensorType FROM Application"
                );
            foreach (var application in result)
            {
                applicationSensorTypeLookup.Add(application.Application, application.SensorType);
            }
        }

        public string GetApplicationSensorType(string application)
        {
            if (applicationSensorTypeLookup == null || !applicationSensorTypeLookup.TryGetValue(application, out string sensorType))
            {
                RefreshLookup();
                sensorType = applicationSensorTypeLookup.GetValueOrDefault(application);
            }
            return sensorType;
        }
    }
}
