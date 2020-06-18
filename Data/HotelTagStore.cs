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
    public class HotelTagStore
    {
        private readonly string _connectionString;
        public HotelTagStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<bool> CheckExistanceAsync(HotelTagKey hotelTagKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync(
                @"SELECT 1 FROM HotelTag WHERE
                    HotelChain = @HotelChain AND 
                    CountryCode = @CountryCode AND
                    Town = @Town AND
                    Suburb = @Suburb AND
                    Tag = @Tag",
                hotelTagKey
            );
            if (result != null)
            {
                return true;
            }
            return false;
        }


        public async Task<IEnumerable<HotelTag>> FindAllByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<HotelTag>(
                @"  SELECT *
                    FROM HotelTag
                    WHERE
                        HotelChain = @HotelChain AND 
                        CountryCode = @CountryCode AND
                        Town = @Town AND
                        Suburb = @Suburb",
                hotelKey
            );
            return result;
        }


        public async Task<HotelTag> FindDetailsAsync(HotelTagKey hotelTagKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync(
                @"SELECT * FROM HotelTag WHERE
                    HotelChain = @HotelChain AND 
                    CountryCode = @CountryCode AND
                    Town = @Town AND
                    Suburb = @Suburb AND
                    Tag = @Tag",
                hotelTagKey
            );
            return result;
        }


        public async Task CreateAsync(HotelTag hotelTag)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "HotelTag_Add_tr",
                hotelTag,
                commandType: CommandType.StoredProcedure);
        }


        // public async Task<HotelLocation> UpdateAsync(int hotelLocationId, HotelLocation hotelLocation)
        // {
        //     var tagsDt = new DataTable();
        //     tagsDt.Columns.Add("Tag", typeof(string));
        //     foreach (string tag in hotelLocation.Tags)
        //     {
        //         tagsDt.Rows.Add(tag);
        //     }

        //     using SqlConnection connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();
        //     var result = await connection.QueryMultipleAsync(
        //         "HotelLocation_Update_sp",
        //         new
        //         {
        //             HotelLocationId = hotelLocationId,
        //             hotelLocation.Name,
        //             TagInput = tagsDt.AsTableValuedParameter("LocationTagInput")
        //         },
        //         commandType: CommandType.StoredProcedure);
        //     var locationResult = await result.ReadFirstAsync<HotelLocation>();
        //     if (locationResult != null)
        //     {
        //         var tags = await result.ReadAsync<string>();
        //         locationResult.Tags = tags.AsList();
        //     }
        //     return locationResult;
        // }


        // public async Task DeleteAsync(int hotelLocationId)
        // {
        //     using SqlConnection connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();
        //     await connection.ExecuteAsync(
        //         "HotelLocation_Delete_sp",
        //         new { HotelLocationId = hotelLocationId },
        //         commandType: CommandType.StoredProcedure
        //     );
        // }
    }
}