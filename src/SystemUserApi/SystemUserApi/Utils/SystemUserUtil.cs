using System.Text.Json;
using SystemUserApi.Constants;
using SystemUserApi.Models;

namespace SystemUserApi.Utils
{
    /// <summary>
    /// Util class for systemuserAPI 
    /// </summary>
    public static class SystemUserUtil
    {

        /// <summary>
        /// Gets the users id
        /// </summary>
        /// <param name="context">the http context</param>
        /// <returns>the logged in system users id</returns>
        public static string? GetSystemUserId(HttpContext context)
        {
            var claim = context.User?.Claims.FirstOrDefault(c => c.Type.Equals(SystemUserConstants.AuthorizationDetails));
            if (claim != null)
            {
                string jwtSystemUSerClaim = claim.Value;

                SystemUserClaim jwtSystemUserClaims = JsonSerializer.Deserialize<SystemUserClaim>(jwtSystemUSerClaim);

                return jwtSystemUserClaims.Systemuser_id[0];
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the system user from the context. Requires that token from Maskinporten is valid and authenticated by the .Net core middleware
        /// </summary>
        /// <param name="context">the http context</param>
        /// <returns>System user object from token</returns>
        public static SystemUserClaim? GetSystemUser(HttpContext? context)
        {
            if (context != null)
            {
                var claim = context.User?.Claims.FirstOrDefault(c => c.Type.Equals(SystemUserConstants.AuthorizationDetails));
                if (claim != null)
                {
                    string jwtSystemUSerClaim = claim.Value;

                    SystemUserClaim? jwtSystemUserClaims = JsonSerializer.Deserialize<SystemUserClaim>(jwtSystemUSerClaim);

                    return jwtSystemUserClaims;
                }
            }

            return null;
        }
    }
}
