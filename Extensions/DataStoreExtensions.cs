using Microsoft.Extensions.DependencyInjection;

using RoomMonitor.Data;

namespace RoomMonitor.Extensions
{
    public static class DataStoreExtensions
    {
        public static void AddDataStores(this IServiceCollection services)
        {
            services.AddScoped<HotelStore>();
            services.AddScoped<HotelTagStore>();
            services.AddScoped<HotelUserStore>();
            services.AddScoped<RoomStore>();
            services.AddScoped<ConfigurationStore>();
            services.AddScoped<SensorApplicationStore>();
            services.AddScoped<MonitorStore>();
            services.AddScoped<SensorDataStore>();
        }
    }
}