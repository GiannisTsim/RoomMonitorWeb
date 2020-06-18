using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Generic;

using RoomMonitor.Models;
using System.Linq;

namespace RoomMonitor.Data
{
    public class SensorDataStore
    {
        private readonly string _connectionString;
        public SensorDataStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<HotelData>> FindReadingsAsync(MonitorRoomAlternateKey monitorRoomAltKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            List<HotelData> result = new List<HotelData>();

            var measureData = await connection.QueryAsync<HotelData, RoomData, MonitorData, ApplicationMeasureData, SensorMeasureData, HotelData>(
                @"  SELECT  HotelChain, 
                            CountryCode, 
                            Town, 
                            Suburb, 
                            RoomType, 
                            Room, 
                            Monitor, 
                            ApplicationMeasure AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingMeasure
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND Room = @Room
                        AND RoomType = @RoomType
                        AND Monitor = @Monitor
                    ORDER BY ReadingDtm",
                (hotel, room, monitor, application, sensorData) =>
                {
                    HotelKey hotelKey = new HotelKey
                    {
                        HotelChain = hotel.HotelChain,
                        CountryCode = hotel.CountryCode,
                        Town = hotel.Town,
                        Suburb = hotel.Suburb
                    };

                    HotelData hotelData = result.FirstOrDefault(h => h.HotelChain == hotel.HotelChain && h.CountryCode == hotel.CountryCode && h.Town == hotel.Town && h.Suburb == hotel.Suburb);
                    if (hotelData == null)
                    {
                        result.Add(hotel);
                        hotelData = hotel;
                        hotelData.RoomData = new List<RoomData>();
                    }
                    RoomData roomData = hotelData.RoomData.FirstOrDefault(r => r.RoomType == room.RoomType && r.Room == room.Room);
                    if (roomData == null)
                    {
                        hotelData.RoomData.Add(room);
                        roomData = room;
                        roomData.MonitorData = new List<MonitorData>();
                    }
                    MonitorData monitorData = roomData.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        roomData.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationMeasureData applicationMeasureData = monitorData.ApplicationMeasureData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationMeasureData == null)
                    {
                        monitorData.ApplicationMeasureData.Add(application);
                        applicationMeasureData = application;
                        applicationMeasureData.SensorData = new List<SensorMeasureData>();
                    }
                    applicationMeasureData.SensorData.Add(sensorData);

                    return hotelData;
                },
                monitorRoomAltKey,
                splitOn: "HotelChain, RoomType, Monitor, Application, ReadingDtm"
            );

            var switchData = await connection.QueryAsync<HotelData, RoomData, MonitorData, ApplicationSwitchData, SensorSwitchData, HotelData>(
                @"  SELECT  HotelChain, 
                            CountryCode, 
                            Town, 
                            Suburb, 
                            RoomType, 
                            Room, 
                            Monitor, 
                            ApplicationSwitch AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingSwitch
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND Room = @Room
                        AND RoomType = @RoomType
                        AND Monitor = @Monitor
                    ORDER BY ReadingDtm",
                (hotel, room, monitor, application, sensorData) =>
                {
                    HotelKey hotelKey = new HotelKey
                    {
                        HotelChain = hotel.HotelChain,
                        CountryCode = hotel.CountryCode,
                        Town = hotel.Town,
                        Suburb = hotel.Suburb
                    };

                    HotelData hotelData = result.FirstOrDefault(h => h.HotelChain == hotel.HotelChain && h.CountryCode == hotel.CountryCode && h.Town == hotel.Town && h.Suburb == hotel.Suburb);
                    if (hotelData == null)
                    {
                        result.Add(hotel);
                        hotelData = hotel;
                        hotelData.RoomData = new List<RoomData>();
                    }
                    RoomData roomData = hotelData.RoomData.FirstOrDefault(r => r.RoomType == room.RoomType && r.Room == room.Room);
                    if (roomData == null)
                    {
                        hotelData.RoomData.Add(room);
                        roomData = room;
                        roomData.MonitorData = new List<MonitorData>();
                    }
                    MonitorData monitorData = roomData.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        roomData.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationSwitchData applicationSwitchData = monitorData.ApplicationSwitchData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationSwitchData == null)
                    {
                        monitorData.ApplicationSwitchData.Add(application);
                        applicationSwitchData = application;
                        applicationSwitchData.SensorData = new List<SensorSwitchData>();
                    }
                    applicationSwitchData.SensorData.Add(sensorData);

                    return hotelData;
                },
                monitorRoomAltKey,
                splitOn: "HotelChain, RoomType, Monitor, Application, ReadingDtm"
            );

            return result;
        }


        public async Task<HotelData> FindReadingsByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            HotelData result = new HotelData()
            {
                HotelChain = hotelKey.HotelChain,
                CountryCode = hotelKey.CountryCode,
                Town = hotelKey.Town,
                Suburb = hotelKey.Suburb,
                RoomData = new List<RoomData>()
            };

            var measureData = await connection.QueryAsync<RoomData, MonitorData, ApplicationMeasureData, SensorMeasureData, RoomData>(
                @"  SELECT  RoomType, 
                            Room, 
                            Monitor, 
                            ApplicationMeasure AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingMeasure
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                    ORDER BY ReadingDtm",
                (room, monitor, application, sensorData) =>
                {
                    RoomData roomData = result.RoomData.FirstOrDefault(r => r.RoomType == room.RoomType && r.Room == room.Room);
                    if (roomData == null)
                    {
                        result.RoomData.Add(room);
                        roomData = room;
                        roomData.MonitorData = new List<MonitorData>();
                    }
                    MonitorData monitorData = roomData.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        roomData.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationMeasureData applicationMeasureData = monitorData.ApplicationMeasureData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationMeasureData == null)
                    {
                        monitorData.ApplicationMeasureData.Add(application);
                        applicationMeasureData = application;
                        applicationMeasureData.SensorData = new List<SensorMeasureData>();
                    }
                    applicationMeasureData.SensorData.Add(sensorData);

                    return roomData;
                },
                hotelKey,
                splitOn: "Monitor, Application, ReadingDtm"
            );

            var switchData = await connection.QueryAsync<RoomData, MonitorData, ApplicationSwitchData, SensorSwitchData, RoomData>(
                @"  SELECT  RoomType, 
                            Room, 
                            Monitor, 
                            ApplicationSwitch AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingSwitch
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                    ORDER BY ReadingDtm",
                (room, monitor, application, sensorData) =>
                {
                    RoomData roomData = result.RoomData.FirstOrDefault(r => r.RoomType == room.RoomType && r.Room == room.Room);
                    if (roomData == null)
                    {
                        result.RoomData.Add(room);
                        roomData = room;
                        roomData.MonitorData = new List<MonitorData>();
                    }
                    MonitorData monitorData = roomData.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        roomData.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationSwitchData applicationSwitchData = monitorData.ApplicationSwitchData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationSwitchData == null)
                    {
                        monitorData.ApplicationSwitchData.Add(application);
                        applicationSwitchData = application;
                        applicationSwitchData.SensorData = new List<SensorSwitchData>();
                    }
                    applicationSwitchData.SensorData.Add(sensorData);

                    return roomData;
                },
                hotelKey,
                splitOn: "Monitor, Application, ReadingDtm"
            );

            return result;
        }



        public async Task<RoomData> FindReadingsByRoomAsync(RoomKey roomKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            RoomData result = new RoomData()
            {
                RoomType = roomKey.RoomType,
                Room = roomKey.Name,
                MonitorData = new List<MonitorData>()
            };

            var measureData = await connection.QueryAsync<MonitorData, ApplicationMeasureData, SensorMeasureData, MonitorData>(
                @"  SELECT  Monitor, 
                            ApplicationMeasure AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingMeasure
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND RoomType = @RoomType
                        AND Room = @Name
                    ORDER BY ReadingDtm",
                (monitor, application, sensorData) =>
                {
                    MonitorData monitorData = result.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        result.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationMeasureData applicationMeasureData = monitorData.ApplicationMeasureData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationMeasureData == null)
                    {
                        monitorData.ApplicationMeasureData.Add(application);
                        applicationMeasureData = application;
                        applicationMeasureData.SensorData = new List<SensorMeasureData>();
                    }
                    applicationMeasureData.SensorData.Add(sensorData);

                    return monitorData;
                },
                roomKey,
                splitOn: "Application, ReadingDtm"
            );

            var switchData = await connection.QueryAsync<MonitorData, ApplicationSwitchData, SensorSwitchData, MonitorData>(
                @"  SELECT  Monitor, 
                            ApplicationSwitch AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingSwitch
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND RoomType = @RoomType
                        AND Room = @Name
                    ORDER BY ReadingDtm",
                (monitor, application, sensorData) =>
                {
                    MonitorData monitorData = result.MonitorData.FirstOrDefault(m => m.Monitor == monitor.Monitor);
                    if (monitorData == null)
                    {
                        result.MonitorData.Add(monitor);
                        monitorData = monitor;
                        monitorData.ApplicationMeasureData = new List<ApplicationMeasureData>();
                        monitorData.ApplicationSwitchData = new List<ApplicationSwitchData>();
                    }
                    ApplicationSwitchData applicationSwitchData = monitorData.ApplicationSwitchData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationSwitchData == null)
                    {
                        monitorData.ApplicationSwitchData.Add(application);
                        applicationSwitchData = application;
                        applicationSwitchData.SensorData = new List<SensorSwitchData>();
                    }
                    applicationSwitchData.SensorData.Add(sensorData);

                    return monitorData;
                },
                roomKey,
                splitOn: "Application, ReadingDtm"
            );

            return result;
        }


        public async Task<MonitorData> FindReadingsByMonitorAsync(MonitorRoomAlternateKey monitorRoomAltKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            MonitorData result = new MonitorData()
            {
                Monitor = monitorRoomAltKey.Monitor,
                ApplicationMeasureData = new List<ApplicationMeasureData>(),
                ApplicationSwitchData = new List<ApplicationSwitchData>()
            };

            var measureData = await connection.QueryAsync<ApplicationMeasureData, SensorMeasureData, ApplicationMeasureData>(
                @"  SELECT  ApplicationMeasure AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingMeasure
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND RoomType = @RoomType
                        AND Room = @Room
                        AND Monitor = @Monitor
                    ORDER BY ReadingDtm",
                (application, sensorData) =>
                {
                    ApplicationMeasureData applicationMeasureData = result.ApplicationMeasureData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationMeasureData == null)
                    {
                        result.ApplicationMeasureData.Add(application);
                        applicationMeasureData = application;
                        applicationMeasureData.SensorData = new List<SensorMeasureData>();
                    }
                    applicationMeasureData.SensorData.Add(sensorData);

                    return applicationMeasureData;
                },
                monitorRoomAltKey,
                splitOn: "ReadingDtm"
            );

            var switchData = await connection.QueryAsync<ApplicationSwitchData, SensorSwitchData, ApplicationSwitchData>(
                @"  SELECT  ApplicationSwitch AS Application, 
                            ReadingDtm, 
                            [Value]
                    FROM ReadingSwitch
                    WHERE HotelChain = @HotelChain
                        AND CountryCode = @CountryCode
                        AND Town = @Town
                        AND Suburb = @Suburb
                        AND RoomType = @RoomType
                        AND Room = @Room
                        AND Monitor = @Monitor
                    ORDER BY ReadingDtm",
                (application, sensorData) =>
                {
                    ApplicationSwitchData applicationSwitchData = result.ApplicationSwitchData.FirstOrDefault(a => a.Application == application.Application);
                    if (applicationSwitchData == null)
                    {
                        result.ApplicationSwitchData.Add(application);
                        applicationSwitchData = application;
                        applicationSwitchData.SensorData = new List<SensorSwitchData>();
                    }
                    applicationSwitchData.SensorData.Add(sensorData);

                    return applicationSwitchData;
                },
                monitorRoomAltKey,
                splitOn: "ReadingDtm"
            );

            return result;
        }
    }
}
