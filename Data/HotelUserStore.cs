using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Generic;

using RoomMonitor.Models;

namespace RoomMonitor.Data
{
    public class HotelUserStore
    {
        private readonly string _connectionString;
        public HotelUserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ApplicationUser>> FindAllHotelUsersAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<ApplicationUser>(
                        @"SELECT *, PersonType.PersonType AS Role FROM Person 
                            INNER JOIN PersonType ON Person.PersonTypeCode = PersonType.PersonTypeCode
                            INNER JOIN HotelUser ON PersonId = HotelUserId"
                    );
            return result;
        }

        public async Task<IEnumerable<ApplicationUser>> FindHotelUsersByHotelAsync(HotelKey hotelKey)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<ApplicationUser>(
                        @"SELECT *, PersonType.PersonType AS Role FROM Person 
                            INNER JOIN PersonType ON Person.PersonTypeCode = PersonType.PersonTypeCode
                            INNER JOIN HotelUser ON PersonId = HotelUserId
                            WHERE HotelChain = @HotelChain AND 
                                CountryCode = @CountryCode AND
                                Town = @Town AND
                                Suburb = @Suburb",
                        hotelKey
                    );
            return result;
        }


        public async Task<ApplicationUser> FindHotelUserDetailsAsync(HotelKey hotelKey, int hotelUserId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                @"SELECT *, PersonType.PersonType AS Role FROM Person 
                            INNER JOIN PersonType ON Person.PersonTypeCode = PersonType.PersonTypeCode
                            INNER JOIN HotelUser ON PersonId = HotelUserId
                            WHERE PersonId = @HotelUserId AND
                                HotelChain = @HotelChain AND 
                                CountryCode = @CountryCode AND
                                Town = @Town AND
                                Suburb = @Suburb",
                new { HotelUserId = hotelUserId, hotelKey.HotelChain, hotelKey.CountryCode, hotelKey.Town, hotelKey.Suburb }
            );
            return result;
        }


        public async Task DeleteHotelUserAsync(HotelKey hotelKey, int hotelUserId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "HotelUser_Drop_tr",
                new { HotelUserId = hotelUserId, hotelKey.HotelChain, hotelKey.CountryCode, hotelKey.Town, hotelKey.Suburb },
                commandType: CommandType.StoredProcedure);
        }

    }
}