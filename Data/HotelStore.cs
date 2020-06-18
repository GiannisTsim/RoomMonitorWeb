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
    public class HotelStore
    {
        private readonly string _connectionString;
        public HotelStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CheckExistanceAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync(
                @"SELECT 1 FROM Hotel WHERE 
                    HotelChain = @HotelChain AND 
                    CountryCode = @CountryCode AND
                    Town = @Town AND
                    Suburb = @Suburb",
                hotelKey
            );
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Hotel>> FindAllAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Hotel>("SELECT * FROM Hotel");
            return result;
        }

        public async Task<Hotel> FindDetailsAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<Hotel>(
                @"SELECT * FROM Hotel WHERE 
                    HotelChain = @HotelChain AND 
                    CountryCode = @CountryCode AND
                    Town = @Town AND
                    Suburb = @Suburb",
                hotelKey
                );
            return result;
        }

        public async Task CreateAsync(Hotel hotel)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Hotel_Add_tr",
                hotel,
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(HotelKey hotelKey, Hotel hotel)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Hotel_Modify_tr",
                new
                {
                    hotelKey.HotelChain,
                    hotelKey.CountryCode,
                    hotelKey.Town,
                    hotelKey.Suburb,
                    NewHotelChain = hotel.HotelChain,
                    NewCountryCode = hotel.CountryCode,
                    NewTown = hotel.Town,
                    NewSuburb = hotel.Suburb,
                    NewNumStar = hotel.NumStar
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "Hotel_Drop_tr",
                hotelKey,
                commandType: CommandType.StoredProcedure);
        }
    }
}