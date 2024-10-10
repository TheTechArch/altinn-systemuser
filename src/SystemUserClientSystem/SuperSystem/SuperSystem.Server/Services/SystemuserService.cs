using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartCloud.Server.Services
{
    public class SystemuserService : ISystemUser
    {
        private readonly HttpClient _client;

        private readonly SystemRegisterConfig _systemRegisterConfig;
     
        public SystemuserService(HttpClient httpClient, IOptions<SystemRegisterConfig> systemRegisterConfig)
        {
            _client = httpClient;
            _systemRegisterConfig = systemRegisterConfig.Value;
        }

        public async Task<CreateRequestSystemUserResponse> CreateSystemUserRequest(CreateRequestSystemUser registerSystemRequest, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.PostAsJsonAsync(_systemRegisterConfig.BaseAdress + _systemRegisterConfig.RequestSystemUserPath, registerSystemRequest);

            if (!httpResponse.IsSuccessStatusCode)
            {

                string body = await httpResponse.Content.ReadAsStringAsync();

            }

            return await httpResponse.Content.ReadFromJsonAsync<CreateRequestSystemUserResponse>();
        }

        public async Task<List<CreateRequestSystemUserResponse>> GetRequests(string systemid, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.GetAsync($"https://platform.at22.altinn.cloud/authentication/api/v1/systemuser/request/vendor/{systemid}");
            return await httpResponse.Content.ReadFromJsonAsync<List<CreateRequestSystemUserResponse>>();
        }

        public async Task<CreateRequestSystemUserResponse> GetRequestStatus(string requestId, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.GetAsync($"https://platform.at22.altinn.cloud/authentication/api/v1/systemuser/request/vendor/{requestId}");
            return await httpResponse.Content.ReadFromJsonAsync<CreateRequestSystemUserResponse>();
        }
    }
}
