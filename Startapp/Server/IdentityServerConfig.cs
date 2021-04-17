using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Startapp.Shared.Core;
using System.Collections.Generic;
using ExtensionGrantModels = IdentityServer.ExtensionGrant.Delegation.Models;

namespace Startapp.Server
{
    public class IdentityServerConfig
    {
        public const string ApiName = "startapp_api";
        public const string ApiFriendlyName = "Startapp API";
        public const string StartAppClientID = "startapp_spa";
        public const string SwaggerClientID = "swaggerui";

        // Identity resources (used by UserInfo endpoint).
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource(ScopeConstants.Roles, new List<string> { JwtClaimTypes.Role })
            };
        }

        //Api resources.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApiName) {                   
                    UserClaims = {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.Role,
                        ClaimConstants.Permission,
                        ScopeConstants.Roles,
                        ApiName
                    }
                }
            };
        }

        // Api scopes.
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(ApiName) {
                    UserClaims = {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.Role,
                        ClaimConstants.Permission,
                        ScopeConstants.Roles,
                        ApiName
                    }
                }
            };
        }

        // Clients want to access resources.
        public static IEnumerable<IdentityServer4.Models.Client> GetClients()
        {
            // Clients credentials.
            return new List<IdentityServer4.Models.Client>
            {
                // http://docs.identityserver.io/en/release/reference/client.html.
                new IdentityServer4.Models.Client
                {
                    ClientId = StartAppClientID,
                    AllowedGrantTypes = new[] { GrantType.ResourceOwnerPassword, ExtensionGrantModels.GrantType.Delegation},
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        ScopeConstants.Roles,
                        ApiName
                    },
                    AllowOfflineAccess = true, // For refresh token.
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,                   
                    //AccessTokenLifetime = 90, // Lifetime of access token in seconds.
                    //AbsoluteRefreshTokenLifetime = 7200,
                    //SlidingRefreshTokenLifetime = 900,
                },

                new IdentityServer4.Models.Client
                {
                    ClientId = SwaggerClientID,
                    ClientName = "Swagger UI",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,

                    AllowedScopes = {
                        ApiName
                    }
                }
            };
        }


    }
}
