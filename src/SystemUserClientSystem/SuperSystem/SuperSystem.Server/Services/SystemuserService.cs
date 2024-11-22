using Microsoft.Extensions.Options;
using smartcloud.server.Models;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;
using System.Net.Http.Headers;

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

        public async Task<List<SystemUser>> GetSystemUsersForSystem(string systemid, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.GetAsync($"{_systemRegisterConfig.BaseAdress}{_systemRegisterConfig.SystemUserListForSystemPath}{systemid}");
            
            SystemUserWrapper? systemUserWrapper = await httpResponse.Content.ReadFromJsonAsync<SystemUserWrapper>();
            return systemUserWrapper?.Data;
        }

        public async Task<CreateRequestSystemUserResponse> GetRequestStatus(string requestId, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.GetAsync($"{_systemRegisterConfig.BaseAdress}{_systemRegisterConfig.RequestSystemUserPath}/{requestId}");
            return await httpResponse.Content.ReadFromJsonAsync<CreateRequestSystemUserResponse>();
        }
    }
}
