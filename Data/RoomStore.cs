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
    public class RoomStore
    {
        private readonly string _connectionString;
        public RoomStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<bool> CheckExistanceAsync(RoomKey roomKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync(
                @"SELECT 1 FROM Room WHERE
                    HotelChain = @HotelChain AND 
                    CountryCode = @CountryCode AND
                    Town = @Town AND
                    Suburb = @Suburb AND
                    RoomType = @RoomType AND
                    Room = @Name",
                roomKey
            );
            if (result != null)
            {
                return true;
            }
            return false;
        }


        public async Task<IEnumerable<Room>> FindAllByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            var lookup = new Dictionary<Tuple<string, string>, Room>();

            await connection.OpenAsync();
            var result = await connection.QueryAsync<Room, dynamic, Room>(
                @"  SELECT 
                         r.*, r.Room as Name, t.Tag 
                    FROM 
                        Room r
                    LEFT JOIN 
                        RoomTag t
                    ON 
                        r.HotelChain = t.HotelChain AND
                        r.CountryCode = t.CountryCode AND
                        r.Town = t.Town AND
                        r.Suburb = t.Suburb AND
                        r.RoomType = t.RoomType AND
                        r.Room = t.Room
                    WHERE 
                        r.HotelChain = @HotelChain AND 
                        r.CountryCode = @CountryCode AND
                        r.Town = @Town AND
                        r.Suburb = @Suburb",
                (r, t) =>
                {
                    var key = Tuple.Create(r.RoomType, r.Name);
                    if (!lookup.TryGetValue(key, out Room room))
                        lookup.Add(key, room = r);
                    if (room.Tags == null)
                        room.Tags = new List<string>();
                    if (t != null && t.Tag != null)
                        room.Tags.Add(t.Tag);
                    return room;
                },
                hotelKey,
                splitOn: "Tag"
            );
            return lookup.Values;
        }


        public async Task<IEnumerable<RoomType>> FindAllRoomTypesAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<RoomType>(
                "  SELECT *, RoomType as Name FROM RoomType"
            );
            return result;
        }


        public async Task<Room> FindDetailsAsync(RoomKey roomKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var roomResult = await connection.QuerySingleOrDefaultAsync<Room>(
                @"SELECT *, Room as Name
                  FROM Room 
                  WHERE
                        HotelChain = @HotelChain AND 
                        CountryCode = @CountryCode AND
                        Town = @Town AND
                        Suburb = @Suburb AND
                        RoomType = @RoomType AND
                        Room = @Name",
                roomKey
                );
            if (roomResult != null)
            {
                var tags = await connection.QueryAsync<string>(
                    @"  SELECT Tag 
                        FROM RoomTag 
                        WHERE
                            HotelChain = @HotelChain AND 
                            CountryCode = @CountryCode AND
                            Town = @Town AND
                            Suburb = @Suburb AND
                            RoomType = @RoomType AND
                            Room = @Name",
                    roomKey
                    );

                roomResult.Tags = tags.AsList();
            }
            return roomResult;
        }


        public async Task CreateAsync(Room room)
        {
            var tagsDt = new DataTable();
            tagsDt.Columns.Add("Tag", typeof(string));
            foreach (string tag in room.Tags)
            {
                tagsDt.Rows.Add(tag);
            }

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Room_Add_tr",
                new
                {
                    room.HotelChain,
                    room.CountryCode,
                    room.Town,
                    room.Suburb,
                    room.RoomType,
                    Room = room.Name,
                    RoomTagTVP = tagsDt.AsTableValuedParameter("TagTableType")
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(RoomKey roomKey, Room room)
        {
            var tagsDt = new DataTable();
            tagsDt.Columns.Add("Tag", typeof(string));
            foreach (string tag in room.Tags)
            {
                tagsDt.Rows.Add(tag);
            }

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Room_Modify_tr",
                new
                {
                    roomKey.HotelChain,
                    roomKey.CountryCode,
                    roomKey.Town,
                    roomKey.Suburb,
                    roomKey.RoomType,
                    Room = roomKey.Name,
                    NewRoomType = room.RoomType,
                    NewRoom = room.Name,
                    NewRoomTagTVP = tagsDt.AsTableValuedParameter("TagTableType")
                },
                commandType: CommandType.StoredProcedure);
        }


        public async Task DeleteAsync(RoomKey roomKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Room_Drop_tr",
                new
                {
                    roomKey.HotelChain,
                    roomKey.CountryCode,
                    roomKey.Town,
                    roomKey.Suburb,
                    roomKey.RoomType,
                    Room = roomKey.Name
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}