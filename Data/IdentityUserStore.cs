using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using RoomMonitor.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace RoomMonitor.Data
{
    public class IdentityUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {

        private readonly string _connectionString;
        public IdentityUserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var p = new DynamicParameters();
            // p.Add("PersonId", user.UserId, DbType.Int32, ParameterDirection.Output);
            p.Add("PersonId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("Email", user.Email, DbType.String, ParameterDirection.Input);
            p.Add("NormalizedEmail", user.NormalizedEmail, DbType.String, ParameterDirection.Input);
            p.Add("PasswordHash", user.PasswordHash, DbType.String, ParameterDirection.Input);
            p.Add("SecurityStamp", user.SecurityStamp, DbType.String, ParameterDirection.Input);

            try
            {

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    if (user.Role == RoomMonitorConstants.UserRoles.SystemAdmin)
                    {
                        await connection.ExecuteAsync(
                            "SystemAdmin_Add_tr",
                            p,
                            commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        p.Add("HotelChain", user.HotelChain, DbType.String, ParameterDirection.Input);
                        p.Add("CountryCode", user.CountryCode, DbType.String, ParameterDirection.Input);
                        p.Add("Town", user.Town, DbType.String, ParameterDirection.Input);
                        p.Add("Suburb", user.Suburb, DbType.String, ParameterDirection.Input);

                        if (user.Role == RoomMonitorConstants.UserRoles.HotelAdmin)
                        {
                            await connection.ExecuteAsync(
                                "HotelAdmin_Add_tr",
                                p,
                                commandType: CommandType.StoredProcedure);
                        }
                        else if (user.Role == RoomMonitorConstants.UserRoles.HotelEmployee)
                        {
                            await connection.ExecuteAsync(
                                "HotelEmployee_Add_tr",
                                p,
                                commandType: CommandType.StoredProcedure);
                        }
                    }

                }
                int personId = p.Get<int>("PersonId");
                if (personId == null)
                    return IdentityResult.Failed();
                // return IdentityResult.Failed(new IdentityError { Code = nameof(CreateAsync), Description = "Unknown User Type" });
                user.PersonId = personId;
                return IdentityResult.Success;
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Error: " + ex.Errors[i].Number + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Console.WriteLine(errorMessages.ToString());
                return IdentityResult.Failed(new IdentityError { Code = nameof(CreateAsync), Description = $"User with email {user.Email} could not be inserted" });
            }

        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Int32.TryParse(userId, out var id))
            {
                return await Task.FromResult<ApplicationUser>(null);
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                @"SELECT *, PersonType AS [Role]
                FROM Person 
                INNER JOIN PersonType
                ON Person.PersonTypeCode = PersonType.PersonTypeCode
                LEFT JOIN HotelUser
                ON PersonId = HotelUserId
                WHERE PersonId = @PersonId",
                new { PersonId = id });
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            ApplicationUser user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                @"SELECT *, PersonType AS [Role]
                FROM Person 
                INNER JOIN PersonType
                ON Person.PersonTypeCode = PersonType.PersonTypeCode
                LEFT JOIN HotelUser
                ON PersonId = HotelUserId
                WHERE NormalizedEmail = @NormalizedEmail",
                new { NormalizedEmail = normalizedUserName });
            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PersonId.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedEmail = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.Email = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string query = "Person_Modify_tr";

            var p = new DynamicParameters();
            p.Add("PersonId", user.PersonId, DbType.Int32, ParameterDirection.Input);
            p.Add("Email", user.Email, DbType.String, ParameterDirection.Input);
            p.Add("NormalizedEmail", user.NormalizedEmail, DbType.String, ParameterDirection.Input);
            p.Add("PasswordHash", user.PasswordHash, DbType.String, ParameterDirection.Input);
            p.Add("SecurityStamp", user.SecurityStamp, DbType.String, ParameterDirection.Input);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    await connection.ExecuteScalarAsync(query, p, commandType: CommandType.StoredProcedure);
                }
                return IdentityResult.Success;
            }
            catch (SqlException ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = nameof(UpdateAsync), Description = $"User with email {user.Email} could not be updated" });
            }
        }

    }
}
