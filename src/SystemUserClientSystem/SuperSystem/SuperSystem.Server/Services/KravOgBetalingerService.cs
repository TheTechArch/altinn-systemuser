using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using IO.Swagger.Model;
using Microsoft.Extensions.Options;
using SuperSystem.Server.Config;
using SuperSystem.Server.Services.Interfaces;
using System.Net.Http.Headers;

namespace SuperSystem.Server.Services
{
    /// <summary>
    /// Service responsible for implementing the IKravOgBetalinger interface
    /// Based on documentation here https://skatteetaten.github.io/api-dokumentasjon/api/kravogbetalinger
    /// </summary>
    public class KravOgBetalingerService : IKravOgBetalinger
    {
        private readonly HttpClient _client;
        private readonly IMaskinportenService _maskinportenService;
        private readonly MaskinportenConfig _maskinportenConfig;
        private readonly KravOgBetalingerConfig _kravOgBetalingerConfig;

        public KravOgBetalingerService(HttpClient httpClient, IMaskinportenService maskinportenService, IOptions<MaskinportenConfig> maskinportenConfig, IOptions<KravOgBetalingerConfig> kravOgBetalingerConfig)
        {
           _client = httpClient;
            _maskinportenService = maskinportenService;
            _kravOgBetalingerConfig = kravOgBetalingerConfig.Value;
            _maskinportenConfig = maskinportenConfig.Value;
        }

        public async Task<AapneKrav?> GetAapneKrav(string orgno, string? from, string? to, string? correlation)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test",_maskinportenConfig.ClientId, _kravOgBetalingerConfig.Scope, orgno);
            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
             AapneKrav? aapneKrav = await _client.GetFromJsonAsync<AapneKrav>(BuildUrl(_kravOgBetalingerConfig.BaseAddress+$"{orgno}/aapnekrav", BuildQueryParams(from, to, correlation)));

            return aapneKrav;
        }

        public async Task<Innbetalinger?> GetInnbetalinger(string orgno, string? from, string? to, string? correlation)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _kravOgBetalingerConfig.Scope, orgno);
            
            if(tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            Innbetalinger? innbetalinger = await _client.GetFromJsonAsync<Innbetalinger>(BuildUrl(_kravOgBetalingerConfig.BaseAddress + $"{orgno}/innbetalinger", BuildQueryParams(from, to, correlation)));

            return innbetalinger;
        }

        public async Task<Krav?> GetKrav(string orgno, string from, string? to, string? correlation)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _kravOgBetalingerConfig.Scope, orgno);
            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            Krav? krav = await _client.GetFromJsonAsync<Krav>(BuildUrl(_kravOgBetalingerConfig.BaseAddress + $"{orgno}/krav", BuildQueryParams(from, to, correlation)));

            return krav;
        }

        public async Task<Utbetalinger?> GetUtbetalinger(string orgno, string? from, string? to, string? correlation)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _kravOgBetalingerConfig.Scope, orgno);
            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            Utbetalinger? utbetalinger = await _client.GetFromJsonAsync<Utbetalinger>(BuildUrl(_kravOgBetalingerConfig.BaseAddress + $"{orgno}/utbetalinger", BuildQueryParams(from, to, correlation)));

            return utbetalinger;
        }


        private static string BuildUrl(string baseUrl, Dictionary<string, string> queryParams)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value; // Add or update parameters
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        private Dictionary<string, string> BuildQueryParams(string? from, string? to, string? correlation)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();  

            if(!string.IsNullOrEmpty(from))
            {
                queryParams.Add("fraOgMed", from);
            }

            if(!string.IsNullOrEmpty(to))
            {
                queryParams.Add("tilOgMed", to);
            }

            if(!string.IsNullOrEmpty(correlation))
            {
                queryParams.Add("Korrelasjonsid", correlation);
            }

            return queryParams;
        }
    }
}
