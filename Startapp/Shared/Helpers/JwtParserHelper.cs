using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Startapp.Shared.Helpers
{
    public static class JwtParserHelper
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            if (jwt.Length > 0)
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
                var roles = claims.Where(c => c.Type == "role").ToList();
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        claims.Remove(role);
                        if (role.Value.ToString().Trim().StartsWith("["))
                        {
                            var parsedRoles = JsonSerializer.Deserialize<string[]>(role.Value.ToString());

                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Value));
                        }
                    }
                }
                var permissions = claims.Where(c => c.Type == "permission").ToList();
                if (permissions != null)
                {
                    foreach (var permission in permissions)
                    {
                        claims.Remove(permission);
                        if (permission.Value.ToString().Trim().StartsWith("["))
                        {
                            var parsedPermissions = JsonSerializer.Deserialize<string[]>(permission.Value.ToString());

                            foreach (var parsedPermission in parsedPermissions)
                            {
                                claims.Add(new Claim("permission", parsedPermission));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim("permission", permission.Value));
                        }
                    }
                }


            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }


    }
}
