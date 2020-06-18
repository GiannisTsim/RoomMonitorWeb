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
    public class SensorApplicationStore
    {
        private readonly string _connectionString;
        public SensorApplicationStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<IEnumerable<ApplicationMeasure>> FindAllApplicationMeasureAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            IEnumerable<ApplicationMeasure> result = await connection.QueryAsync<ApplicationMeasure>(
                @"SELECT    a.Application, 
                            a.Name, 
                            a.Description,
                            m.UnitMeasure,
                            lmin.LimitValue AS LimitMin,
                            lmax.LimitValue AS LimitMax,
                            dmin.LimitValue AS DefaultMin,
                            dmax.LimitValue AS DefaultMax
                    FROM Application a
                    INNER JOIN ApplicationMeasure m
                    ON a.Application = m.ApplicationMeasure
                    LEFT JOIN ApplicationLimit lmin
                    ON m.ApplicationMeasure = lmin.ApplicationMeasure AND lmin.LimitType='LimitMin'
                    LEFT JOIN ApplicationLimit lmax
                    ON m.ApplicationMeasure = lmax.ApplicationMeasure AND lmax.LimitType='LimitMax'
                    LEFT JOIN ApplicationLimit dmin
                    ON m.ApplicationMeasure = dmin.ApplicationMeasure AND dmin.LimitType='DefaultMin'
                    LEFT JOIN ApplicationLimit dmax
                    ON m.ApplicationMeasure = dmax.ApplicationMeasure AND dmax.LimitType='DefaultMax'"
            );
            return result;
        }


        public async Task<IEnumerable<ApplicationSwitch>> FindAllApplicationSwitchAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            IEnumerable<ApplicationSwitch> result = await connection.QueryAsync<ApplicationSwitch>(
                @"SELECT    a.Application, 
                            a.Name, 
                            a.Description, 
                            s.Value_0, 
                            s.Value_1
                    FROM Application a
                    INNER JOIN ApplicationSwitch s
                    ON a.Application = s.ApplicationSwitch"
            );
            return result;
        }

        public async Task CreateApplicationMeasureAsync(ApplicationMeasure applicationMeasure)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                   "ApplicationMeasure_Add_tr",
                   new
                   {
                       ApplicationMeasure = applicationMeasure.Application,
                       applicationMeasure.Name,
                       applicationMeasure.Description,
                       applicationMeasure.UnitMeasure,
                       applicationMeasure.LimitMin,
                       applicationMeasure.LimitMax,
                       applicationMeasure.DefaultMin,
                       applicationMeasure.DefaultMax
                   },
                   commandType: CommandType.StoredProcedure);
        }


        public async Task CreateApplicationSwitchAsync(ApplicationSwitch applicationSwitch)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                    "ApplicationSwitch_Add_tr",
                    new
                    {
                        ApplicationSwitch = applicationSwitch.Application,
                        applicationSwitch.Name,
                        applicationSwitch.Description,
                        applicationSwitch.Value_0,
                        applicationSwitch.Value_1
                    },
                    commandType: CommandType.StoredProcedure);
        }
    }
}