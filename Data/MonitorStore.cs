using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Generic;

using RoomMonitor.Models;

namespace RoomMonitor.Data
{
    public class MonitorStore
    {
        private readonly string _connectionString;
        public MonitorStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task CreateAsync(HotelKey hotelKey, MonitorRegistration monitorRegistration)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Monitor_Register_tr",
                new
                {
                    hotelKey.HotelChain,
                    hotelKey.CountryCode,
                    hotelKey.Town,
                    hotelKey.Suburb,
                    monitorRegistration.MACAddress,
                    monitorRegistration.ConfigurationType,
                    monitorRegistration.Manufacturer,
                    monitorRegistration.Model,
                    monitorRegistration.SWVersion,
                    monitorRegistration.SWUpdateDtm
                },
                commandType: CommandType.StoredProcedure);
        }


        public async Task<IEnumerable<MonitorRoom>> FindAllMonitorRoomsByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<MonitorRoom>(
                @"SELECT * FROM MonitorRoom_V 
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb",
                hotelKey
            );
            return result;
        }

        public async Task<IEnumerable<MonitorUnassigned>> FindAllMonitorUnassignedByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<MonitorUnassigned>(
                @"SELECT * FROM MonitorUnassigned_V 
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb",
                hotelKey
            );
            return result;
        }

        public async Task<MonitorRoom> FindMonitorRoomDetailsAsync(MonitorKey monitorKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<MonitorRoom>(
                @"SELECT * FROM MonitorRoom_V 
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb
                    AND MACAddress = @MACAddress",
                monitorKey
            );
            return result;
        }


        public async Task<MonitorUnassigned> FindMonitorUnassignedDetailsAsync(MonitorKey monitorKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<MonitorUnassigned>(
                @"SELECT * FROM MonitorUnassigned_V 
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb
                    AND MACAddress = @MACAddress",
                monitorKey
            );
            return result;
        }



        public async Task<IEnumerable<MonitorRoomAlternateKey>> FindMonitorHistoryByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<MonitorRoomAlternateKey>(
                @"SELECT DISTINCT HotelChain, CountryCode, Town, Suburb, RoomType, Room, Monitor
                    FROM Reading
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb",
                hotelKey
            );
            return result;
        }

        public async Task<IEnumerable<MonitorRoomAlternateKey>> FindMonitorHistoryByRoomAsync(RoomKey roomKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<MonitorRoomAlternateKey>(
                @"SELECT DISTINCT HotelChain, CountryCode, Town, Suburb, RoomType, Room, Monitor
                    FROM Reading
                    WHERE HotelChain = @HotelChain
                    AND CountryCode = @CountryCode
                    AND Town = @Town
                    AND Suburb = @Suburb
                    AND RoomType = @RoomType
                    AND Room = @Name",
                roomKey
            );
            return result;
        }


        public async Task AddMonitorLogAsync(HotelKey hotelKey, string MACAddress, MonitorLog monitorLog)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "MonitorLog_Add_tr",
                new
                {
                    hotelKey.HotelChain,
                    hotelKey.CountryCode,
                    hotelKey.Town,
                    hotelKey.Suburb,
                    MACAddress,
                    monitorLog.LogDtm,
                    monitorLog.BatteryLevel,
                    monitorLog.ConnFail,
                    monitorLog.PostFail,
                    monitorLog.Rssi
                },
                commandType: CommandType.StoredProcedure);
        }


        public async Task AddMonitorReadingAsync(MonitorRoom monitorRoom, MonitorReading reading)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            if (reading is MonitorReadingSwitch readingSwitch)
            {
                await connection.ExecuteAsync(
                    "ReadingSwitch_Add_tr",
                    new
                    {
                        monitorRoom.HotelChain,
                        monitorRoom.CountryCode,
                        monitorRoom.Town,
                        monitorRoom.Suburb,
                        monitorRoom.MACAddress,
                        monitorRoom.RoomType,
                        monitorRoom.Room,
                        monitorRoom.Monitor,
                        ApplicationSwitch = readingSwitch.Application,
                        readingSwitch.ReadingDtm,
                        readingSwitch.Value
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else if (reading is MonitorReadingMeasure readingMeasure)
            {
                await connection.ExecuteAsync(
                    "ReadingMeasure_Add_tr",
                    new
                    {
                        monitorRoom.HotelChain,
                        monitorRoom.CountryCode,
                        monitorRoom.Town,
                        monitorRoom.Suburb,
                        monitorRoom.MACAddress,
                        monitorRoom.RoomType,
                        monitorRoom.Room,
                        monitorRoom.Monitor,
                        ApplicationMeasure = readingMeasure.Application,
                        readingMeasure.ReadingDtm,
                        readingMeasure.Value
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AssignMonitorAsync(MonitorRoom monitorRoom)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Monitor_Assign_tr",
                new
                {
                    monitorRoom.HotelChain,
                    monitorRoom.CountryCode,
                    monitorRoom.Town,
                    monitorRoom.Suburb,
                    monitorRoom.MACAddress,
                    monitorRoom.RoomType,
                    monitorRoom.Room,
                    monitorRoom.Monitor
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeassignMonitorAsync(MonitorKey monitorKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Monitor_Deassign_tr",
                monitorKey,
                commandType: CommandType.StoredProcedure);
        }
    }
}