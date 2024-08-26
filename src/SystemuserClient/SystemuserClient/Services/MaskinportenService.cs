using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Interfaces;

namespace Altinn.ApiClients.Maskinporten.Services
{
    public class MaskinportenService: IMaskinportenService
    {
        private readonly HttpClient _client;

        public MaskinportenService(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = httpClient;
        }

        public async Task<TokenResponse> GetToken(string base64EncodedJwk, string environment, string clientId, string scope, string resource, string systemUserOrgno)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedJwk);
            string jwkjson = Encoding.UTF8.GetString(base64EncodedBytes);
            JsonWebKey jwk = new JsonWebKey(jwkjson);
            return await GetToken(jwk, environment, clientId, scope, resource, systemUserOrgno);
        }

        public async Task<TokenResponse> GetToken(JsonWebKey jwk, string environment, string clientId, string scope, string resource, string systemUserOrgno)
        {
            TokenResponse accesstokenResponse;
            string jwtAssertion = GetJwtAssertion(jwk, environment, clientId, scope, resource, systemUserOrgno);
            FormUrlEncodedContent content = GetUrlEncodedContent(jwtAssertion);
            accesstokenResponse = await PostToken(environment, content);
            return accesstokenResponse;
        }

        /// <summary>
        /// Builds the JWT Grant
        /// </summary>
        /// <param name="jwk">The JSON web key containting the private key used for the Client integration</param>
        /// <param name="environment"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="resource"></param>
        /// <param name="systemUserOrgno"></param>
        /// <returns></returns>
        public string GetJwtAssertion(JsonWebKey jwk, string environment, string clientId, string scope, string resource, string systemUserOrgno)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            JwtHeader header = GetHeader(jwk);

            JwtPayload payload = new JwtPayload
            {
                { "aud", GetAssertionAud(environment) },
                { "scope", scope },
                { "iss", clientId },
                { "exp", dateTimeOffset.ToUnixTimeSeconds() + 10 },
                { "iat", dateTimeOffset.ToUnixTimeSeconds() },
                { "jti", Guid.NewGuid().ToString() },
            };
           
            
            if(systemUserOrgno != null)
            {
                JwtPayload systemUserOrg = new JwtPayload
                {
                    { "authority", "iso6523-actorid-upis" },
                    { "ID", $"0192:{systemUserOrgno}" },
                };

                    JwtPayload authorizationDetail = new JwtPayload()
                {
                    { "systemuser_org", systemUserOrg },
                    { "type" , "urn:altinn:systemuser"}
                };

                    List<JwtPayload> authorizationDetails = new List<JwtPayload>
                {
                    authorizationDetail
                };

                payload.Add("authorization_details", authorizationDetails);
            }


            if (!string.IsNullOrEmpty(resource))
            {
                payload.Add("resource", resource);
            }

            JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(securityToken);
        }

        private JwtHeader GetHeader(JsonWebKey jwk)
        {
            return new JwtHeader(new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256));
        }

        private FormUrlEncodedContent GetUrlEncodedContent(string assertion)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", assertion),
            });

            return formContent;
        }

        public async Task<TokenResponse> PostToken(string environment, FormUrlEncodedContent bearer)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(GetTokenEndpoint(environment)),
                Content = bearer
            };

            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                string successResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TokenResponse>(successResponse);
            }
            
            string errorResponse = await response.Content.ReadAsStringAsync();
            ErrorReponse error = JsonSerializer.Deserialize<ErrorReponse>(errorResponse) ?? new ErrorReponse()
            {
                ErrorType = "deserializing",
                Description = "Unable to deserialize error from Maskinporten. Received: " +
                              (string.IsNullOrEmpty(errorResponse) ? "<empty>" : errorResponse)
            };

            // _logger.LogError("errorType={errorType} description={description} statuscode={statusCode}", error.ErrorType, error.Description, response.StatusCode);
            throw new TokenRequestException(error.Description);
        }

        private string GetAssertionAud(string environment)
        {
            return environment switch
            {
                "prod" => "https://maskinporten.no/",
                "test" => "https://test.maskinporten.no/",
                _ => throw new ArgumentException("Invalid environment setting. Valid values: prod, test")
            };
        }

        private string GetTokenEndpoint(string environment)
        {
            return environment switch
            {
                "prod" => "https://maskinporten.no/token",
                "test" => "https://test.maskinporten.no/token",
                _ => throw new ArgumentException("Invalid environment setting. Valid values: prod, test")
            };
        }
    }
}
