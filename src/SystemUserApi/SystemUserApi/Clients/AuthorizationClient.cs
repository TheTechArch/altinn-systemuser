using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Services.Interfaces;
using System.Net.Http.Headers;
using SystemUserApi.Clients.Interfaces;
using SystemUserApi.Configuration;

namespace SystemUserApi.Clients
{
    public class AuthorizationClient: IAuthorization
    {
        HttpClient _client;
        AuthorizationConfig _authorizationConfig;
        IMaskinportenService _maskinportenService;
        MaskinportenConfig _maskinportenConfig;
        ITokenExchange _tokenExhange;
       
        /// <summary>
        /// Constructor for client
        /// </summary>
        /// <param name="client"></param>
        public AuthorizationClient(HttpClient client, IOptions<AuthorizationConfig> autorizationConfig, IMaskinportenService maskinportenService, IOptions<MaskinportenConfig> maskinportenConfig, ITokenExchange tokenExchange) {
            _client = client;
            _authorizationConfig = autorizationConfig.Value;
            _maskinportenService = maskinportenService;
            _maskinportenConfig = maskinportenConfig.Value;
            _tokenExhange = tokenExchange;
        }

        public async Task<XacmlJsonResponse> AuthorizeReqeust(XacmlJsonRequestRoot xacmlJsonRequestRoot)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _authorizationConfig.PDPScope, null);

            if(tokenResponse == null)
            {
                throw new Exception("Failed to get token");
            }

            string exchangedToken = await _tokenExhange.ExhangeMaskinporten(tokenResponse.AccessToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", exchangedToken);
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _authorizationConfig.ApiKey);
             HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(_authorizationConfig.PDPEndpoint, xacmlJsonRequestRoot);
            responseMessage.EnsureSuccessStatusCode();

            XacmlJsonResponse? xacmlJsonResponse = await responseMessage.Content.ReadFromJsonAsync<XacmlJsonResponse>();
            
            if (xacmlJsonResponse == null)
            {
                throw new Exception("Failed to get response from PDP");
            }

            return xacmlJsonResponse;
        }
    }
}
