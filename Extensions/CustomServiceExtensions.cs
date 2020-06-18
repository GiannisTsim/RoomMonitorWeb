using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using RoomMonitor.Services;

namespace RoomMonitor.Extensions
{
    public static class CustomServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<EmailSender>();

            services.AddSingleton<IAuthorizationHandler, HotelAccessHandler>();

            services.AddScoped<IProfileService, UserProfileService>();

            services.AddSingleton<SensorTypeProvider>();
        }
    }
}