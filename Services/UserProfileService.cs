using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using RoomMonitor.Models;
using System.Text.Json;

namespace RoomMonitor.Services
{
    public class UserProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            switch (context.Client.ClientId)
            {
                case RoomMonitorConstants.ClientIds.SpaClient:
                    claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                    claims.Add(new Claim(JwtClaimTypes.Role, user.Role));

                    if (user.Role == RoomMonitorConstants.UserRoles.HotelAdmin || user.Role == RoomMonitorConstants.UserRoles.HotelEmployee)
                    {
                        // Hotel userHotel = new Hotel
                        // {
                        //     HotelChain = user.HotelChain,
                        //     CountryCode = user.CountryCode,
                        //     Town = user.Town,
                        //     Suburb = user.Suburb
                        // };

                        claims.Add(
                            new Claim(RoomMonitorConstants.ClaimTypes.Hotel,
                            JsonSerializer.Serialize(
                                new { user.HotelChain, user.CountryCode, user.Town, user.Suburb },
                                serializeOptions))
                        );
                        // claims.Add(
                        //     new Claim(RoomMonitorConstants.ClaimTypes.Hotel,
                        //     JsonSerializer.Serialize(
                        //         userHotel,
                        //         serializeOptions))
                        // );
                    }
                    break;

                case RoomMonitorConstants.ClientIds.DeviceClient:
                    claims.Add(
                            new Claim(RoomMonitorConstants.ClaimTypes.Hotel,
                            JsonSerializer.Serialize(
                                new { user.HotelChain, user.CountryCode, user.Town, user.Suburb },
                                serializeOptions))
                        );
                    break;
            }

            context.AddRequestedClaims(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null && !string.IsNullOrEmpty(user.PasswordHash);

            // THe device client can only receive tokens issued for a hotel admin
            if (context.Client.ClientId == RoomMonitorConstants.ClientIds.DeviceClient)
            {
                context.IsActive = context.IsActive && (user.Role == RoomMonitorConstants.UserRoles.HotelAdmin);
            }
        }
    }
}
