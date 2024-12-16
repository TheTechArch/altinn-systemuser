using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.Extensions.Options;
using smartcloud.server.Clients.Interfaces;
using smartcloud.server.Config;
using SmartCloud.Server.Config;
using System.Net;
using System.Net.Http.Headers;
using SystemUserApi.Models.Logistics;
using SystemUserApi.Models.Salary;

namespace smartcloud.server.Clients
{
    public class SalaryClient : ISalary
    {
        HttpClient _client;
        IMaskinportenService _maskinportenService;
        MaskinportenConfig _maskinportenConfig;
        SmartcloudConfig _smartCloudConfig;

        public SalaryClient(HttpClient httpClient, IOptions<MaskinportenConfig> maskinportenConfig, IOptions<SmartcloudConfig> smartCloudConfig, IMaskinportenService maskinportenService)
        {
            _client = httpClient;
            _maskinportenConfig = maskinportenConfig.Value;
            _smartCloudConfig = smartCloudConfig.Value;
            _maskinportenService = maskinportenService;
        }

        public async Task<SalaryInfo?> GetSalaryInfo(string systemUserOrg, string dataOrganization)
        {
            TokenResponse? tokenResponse = await _maskinportenService.GetToken(_maskinportenConfig.EncodedJwk, "test", _maskinportenConfig.ClientId, _smartCloudConfig.LogisticsScope, systemUserOrg);
            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("Failed to get token");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            HttpResponseMessage httpResponse = await _client.GetAsync(_smartCloudConfig.SalariApi + dataOrganization);

            if(httpResponse.StatusCode.Equals(HttpStatusCode.Forbidden))
            {
                return null;
            }

            if(!httpResponse.IsSuccessStatusCode)
            {
                return new SalaryInfo
                {
                     SalaryAmount = (int) httpResponse.StatusCode
         
                };
            }

            SalaryInfo? salaryInfo = await httpResponse.Content.ReadFromJsonAsync<SalaryInfo>();
                
            return salaryInfo;
        }
    }
}
