using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using IO.Swagger.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using smartcloud.server.Clients.Interfaces;
using smartcloud.server.Config;
using SmartCloud.Server.Config;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using SystemUserApi.Models.Logistics;
using SystemUserApi.Models.Salary;

namespace smartcloud.server.Clients
{
    public class LogisticsClient: ILogistics
    {
        HttpClient _client;
        IMaskinportenService _maskinportenService;
        MaskinportenConfig _maskinportenConfig;
        SmartcloudConfig _smartCloudConfig;
       
        public LogisticsClient(HttpClient httpClient, IOptions<MaskinportenConfig> maskinportenConfig, IOptions<SmartcloudConfig> smartCloudConfig, IMaskinportenService maskinportenService)
        {
            _client = httpClient;
            _maskinportenConfig = maskinportenConfig.Value;
            _smartCloudConfig = smartCloudConfig.Value;
            _maskinportenService = maskinportenService;
        }

        public async Task<LogisticStatus?> GetLogicstatus(string systemUserOrg, string dataOrganization)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _smartCloudConfig.LogisticsScope, systemUserOrg);
            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            HttpResponseMessage httpResponse = await _client.GetAsync(_smartCloudConfig.LogisticApi + dataOrganization);

            if (httpResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
            {
                return null;
            }

            if(!httpResponse.IsSuccessStatusCode)
            {
                return new LogisticStatus
                {
                    Status = "Error",
                    ExportStatus = httpResponse.StatusCode.ToString(), 
                };
            }

            LogisticStatus? logisticStatus = await httpResponse.Content.ReadFromJsonAsync<LogisticStatus>();

            return logisticStatus;
        }
    }
}
