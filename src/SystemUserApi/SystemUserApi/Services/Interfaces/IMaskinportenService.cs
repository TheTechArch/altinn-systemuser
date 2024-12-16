using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.IdentityModel.Tokens;

namespace Altinn.ApiClients.Maskinporten.Interfaces
{
    public interface IMaskinportenService
    {
        Task<TokenResponse?> GetToken(string scope);


        /// <summary>
        /// Generates a Maskinporten access token using a JsonWebKey
        /// </summary>
        Task<TokenResponse?> GetToken(JsonWebKey jwk, string environment, string clientId, string scope);

        /// <summary>
        /// Generates a Maskinporten access token using a base64encoded JsonWebKey
        /// </summary>
        Task<TokenResponse?> GetToken(string base64EncodedJWK, string environment, string clientId, string scope);
    }
}
