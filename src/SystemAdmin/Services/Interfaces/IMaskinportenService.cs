using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Altinn.SystemAdmin.Maskinporten.Models;
using Microsoft.IdentityModel.Tokens;

namespace Altinn.SystemAdmin.Maskinporten.Interfaces
{
    public interface IMaskinportenService
    {
        /// <summary>
        /// Generates a Maskinporten access token using a JsonWebKey
        /// </summary>
        Task<TokenResponse?> GetToken(JsonWebKey jwk, string environment, string clientId, string scope, string systemUserOrg);

        /// <summary>
        /// Generates a Maskinporten access token using a base64encoded JsonWebKey
        /// </summary>
        Task<TokenResponse?> GetToken(string scope, string systemUserOrg);
    }
}
