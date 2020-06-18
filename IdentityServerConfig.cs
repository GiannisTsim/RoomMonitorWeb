using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace RoomMonitor
{
    public static class IdentityServerConfig
    {
        // Identity resources are data like user ID, name, or email address of a user. 
        // An identity resource has a unique name, and you can assign arbitrary claim types to it. 
        // These claims will then be included in the identity token for the user. 
        // The client will use the scope parameter to request access to an identity resource.
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var userProfile = new IdentityResource(
                name: RoomMonitorConstants.IdentityResourceNames.UserProfile,
                displayName: "User Profile Data",
                claimTypes: new[] { JwtClaimTypes.Email, JwtClaimTypes.Role, RoomMonitorConstants.ClaimTypes.Hotel }
            );

            var deviceProfile = new IdentityResource(
                name: RoomMonitorConstants.IdentityResourceNames.DeviceProfile,
                displayName: "Device Profile Data",
                claimTypes: new[] { RoomMonitorConstants.ClaimTypes.Hotel }
            );

            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                // new IdentityResources.Profile(),
                userProfile,
                deviceProfile
            };
        }


        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                // new ApiResource(IdentityServerConstants.LocalApi.ScopeName),

                new ApiResource {
                    Name = RoomMonitorConstants.ApiResourceName,

                    Scopes = {
                        new Scope {
                            Name = RoomMonitorConstants.Scopes.UserScope,
                            DisplayName = "User Access to RoomMonitor API",
                            UserClaims = {JwtClaimTypes.Role, RoomMonitorConstants.ClaimTypes.Hotel}
                        },
                        new Scope {
                            Name = RoomMonitorConstants.Scopes.DeviceScope,
                            DisplayName = "Device Access to RoomMonitor API",
                            UserClaims = {RoomMonitorConstants.ClaimTypes.Hotel}
                        }
                    }
                }
            };
        }


        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = RoomMonitorConstants.ClientIds.SpaClient,
                    ClientName = "RoomMonitor Angular SPA Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,

                    AccessTokenLifetime = 3600,

                    RedirectUris =
                    {
                        "http://localhost:5000",
                        "http://localhost:5000/silent-renew.html",
                        "http://localhost:5000/popup.html",
                    },
                    PostLogoutRedirectUris = { "http://localhost:5000" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        RoomMonitorConstants.Scopes.UserScope,
                        RoomMonitorConstants.IdentityResourceNames.UserProfile
                    }
                },

                new Client {
                    ClientId = RoomMonitorConstants.ClientIds.DeviceClient,
                    ClientName = "RoomMonitor Sensor Device Client",

                    AccessTokenLifetime = 3600,
                    IdentityTokenLifetime = 3600,

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        RoomMonitorConstants.Scopes.DeviceScope,
                        RoomMonitorConstants.IdentityResourceNames.DeviceProfile
                    }
                }
            };
        }
    }
}