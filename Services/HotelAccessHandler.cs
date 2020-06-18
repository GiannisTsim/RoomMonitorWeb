using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

using RoomMonitor.AuthorizationRequirements;
using RoomMonitor.Models;

namespace RoomMonitor.Services
{
    public class HotelAccessHandler : AuthorizationHandler<HotelAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelAccessHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       HotelAccessRequirement requirement)
        {


            // Permit if not trying to access hotel resource
            // if (!routeValues.TryGetValue("hotelId", out object routeHotelId))
            // {
            //     context.Succeed(requirement);
            // }

            // Permit if user is System Admin
            if (context.User.FindFirstValue(JwtClaimTypes.Role) == RoomMonitorConstants.UserRoles.SystemAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            // Else check for valid hotel claim (in case of hotel user or device)
            else
            {
                var queryValues = _httpContextAccessor.HttpContext.Request.Query;
                string hotelClaim = context.User.FindFirstValue(RoomMonitorConstants.ClaimTypes.Hotel);
                if (hotelClaim == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                Hotel userHotel = JsonSerializer.Deserialize<Hotel>(
                    hotelClaim,
                    serializeOptions);

                if (userHotel.HotelChain == queryValues["hotelChain"]
                    && userHotel.CountryCode == queryValues["countryCode"]
                    && userHotel.Town == queryValues["town"]
                    && userHotel.Suburb == queryValues["suburb"])
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }

        // protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        //                                                HotelAccessRequirement requirement)
        // {

        //     var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;

        //     // Permit if not trying to access hotel resource
        //     if (!routeValues.TryGetValue("hotelId", out object routeHotelId))
        //     {
        //         context.Succeed(requirement);
        //     }
        //     // Permit if user is System Admin
        //     else if (context.User.FindFirstValue(JwtClaimTypes.Role) == RoomMonitorConstants.UserRoles.SystemAdmin)
        //     {
        //         context.Succeed(requirement);
        //     }
        //     // Else check for valid hotelId claim (in case of hotel user or device)
        //     else
        //     {
        //         string userHotelId = context.User.FindFirstValue(RoomMonitorConstants.ClaimTypes.HotelId);
        //         if (routeHotelId.ToString() == userHotelId)
        //         {
        //             context.Succeed(requirement);
        //         }
        //     }

        //     return Task.CompletedTask;
        // }

    }
}

