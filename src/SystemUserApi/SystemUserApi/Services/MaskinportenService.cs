﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Interfaces;
using SmartCloud.Server.Config;
using Microsoft.Extensions.Options;
using Altinn.ApiClients.Maskinporten.Models;

namespace Altinn.ApiClients.Maskinporten.Services
{
    /// <summary>
    /// This service is responsible for creating Maskinporten tokens to be used for authetication of this application when calling Altinn API
    /// </summary>
    public class MaskinportenService: IMaskinportenService
    {
        private readonly HttpClient _client;
        private readonly MaskinportenConfig _maskinPortenConfig;

        public MaskinportenService(HttpClient httpClient, IOptions<MaskinportenConfig> maskinPortenConfig)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = httpClient;
            _maskinPortenConfig = maskinPortenConfig.Value;
        }


        /// <summary>
        /// Creates a Maskinporten token using a base64 encoded JsonWebKey
        /// </summary>
        /// <param name="base64EncodedJwk"></param>
        /// <param name="environment"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="systemUserOrgno"></param>
        /// <returns></returns>
        public async Task<TokenResponse?> GetToken(string scope)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(_maskinPortenConfig.EncodedJwk);
            string jwkjson = Encoding.UTF8.GetString(base64EncodedBytes);
            JsonWebKey jwk = new JsonWebKey(jwkjson);
            return await GetToken(jwk, _maskinPortenConfig.Environment, _maskinPortenConfig.ClientId, scope);
        }


        /// <summary>
        /// Creates a Maskinporten token using a base64 encoded JsonWebKey
        /// </summary>
        /// <param name="base64EncodedJwk"></param>
        /// <param name="environment"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="systemUserOrgno"></param>
        /// <returns></returns>
        public async Task<TokenResponse?> GetToken(string base64EncodedJwk, string environment, string clientId, string scope)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedJwk);
            string jwkjson = Encoding.UTF8.GetString(base64EncodedBytes);
            JsonWebKey jwk = new JsonWebKey(jwkjson);
            return await GetToken(jwk, environment, clientId, scope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwk"></param>
        /// <param name="environment"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="resource"></param>
        /// <param name="systemUserOrgno"></param>
        /// <returns></returns>
        public async Task<TokenResponse?> GetToken(JsonWebKey jwk, string environment, string clientId, string scope)
        {
            TokenResponse? accesstokenResponse;
            string jwtAssertion = GetJwtAssertion(jwk, environment, clientId, scope);
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
        private string GetJwtAssertion(JsonWebKey jwk, string environment, string clientId, string scope)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);

            /// Create the JWT header with signature using the JWK. This is the code that create proof that client has private key for the Integration in Maskinporten
            JwtHeader header = GetHeader(jwk);

            /// Create the JWT payload in JWT Grant. This is the same content as a regular Maskinporten token
            JwtPayload payload = new JwtPayload
            {
                { "aud", GetAssertionAud(environment) },
                { "scope", scope },
                { "iss", clientId },
                { "exp", dateTimeOffset.ToUnixTimeSeconds() + 10 },
                { "iat", dateTimeOffset.ToUnixTimeSeconds() },
                { "jti", Guid.NewGuid().ToString() },
            };
           
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="bearer"></param>
        /// <returns></returns>
        /// <exception cref="TokenRequestException"></exception>
        private async Task<TokenResponse?> PostToken(string environment, FormUrlEncodedContent bearer)
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

            Console.Write(error.ToString());
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
