namespace RoomMonitor
{
    public static class RoomMonitorConstants
    {

        public const string ApiResourceName = "roomMonitorApi";

        public static class IdentityResourceNames
        {
            public const string UserProfile = "profile.user";
            public const string DeviceProfile = "profile.device";
        }

        public static class ClientIds
        {
            public const string SpaClient = "roomMonitorSpaClient";
            public const string DeviceClient = "roomMonitorDeviceClient";
        }

        public static class Scopes
        {
            public const string UserScope = "roomMonitorApi.user";
            public const string DeviceScope = "roomMonitorApi.device";
        }

        public static class ClaimTypes
        {
            public const string Hotel = "hotel";
        }

        public static class UserRoles
        {
            public const string SystemAdmin = "System Administrator";
            public const string HotelAdmin = "Hotel Administrator";
            public const string HotelEmployee = "Hotel Employee";
        }

        public static class Policies
        {
            public const string GenericUserAccess = "GenericUserAccess";
            public const string SystemAdminAccess = "SystemAdminAccess";
            public const string SystemOrHotelAdminAccess = "SystemOrHotelAdminAccess";
            public const string HotelUserAccess = "HotelUserAccess";
            public const string HotelAdminAccess = "HotelAdminAccess";
            public const string HotelEmployeeAccess = "HotelEmployeeAccess";
            public const string DeviceAccess = "DeviceAccess";
        }
    }
}