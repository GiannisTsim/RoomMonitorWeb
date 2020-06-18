using Microsoft.Extensions.DependencyInjection;
using IdentityModel;
using IdentityServer4;

using RoomMonitor.AuthorizationRequirements;

namespace RoomMonitor.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
                {
                    options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                        {
                            policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                            policy.RequireAuthenticatedUser();
                        }
                    );

                    options.AddPolicy(RoomMonitorConstants.Policies.GenericUserAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.SystemAdminAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.RequireClaim(JwtClaimTypes.Role, RoomMonitorConstants.UserRoles.SystemAdmin);
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.SystemOrHotelAdminAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.RequireAssertion(context =>
                                context.User.IsInRole(RoomMonitorConstants.UserRoles.SystemAdmin) || context.User.IsInRole(RoomMonitorConstants.UserRoles.HotelAdmin)
                        );
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.HotelUserAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.RequireAssertion(context =>
                                context.User.IsInRole(RoomMonitorConstants.UserRoles.HotelAdmin) || context.User.IsInRole(RoomMonitorConstants.UserRoles.HotelEmployee)
                        );
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.HotelAdminAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.RequireClaim(JwtClaimTypes.Role, RoomMonitorConstants.UserRoles.HotelAdmin);
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.HotelEmployeeAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.UserScope);
                        policy.RequireClaim(JwtClaimTypes.Role, RoomMonitorConstants.UserRoles.HotelEmployee);
                        policy.Requirements.Add(new HotelAccessRequirement());
                    });

                    options.AddPolicy(RoomMonitorConstants.Policies.DeviceAccess, policy =>
                    {
                        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                        policy.RequireClaim(JwtClaimTypes.Scope, RoomMonitorConstants.Scopes.DeviceScope);
                    });
                });
        }
    }
}