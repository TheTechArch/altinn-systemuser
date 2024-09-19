using SuperSystem.Server.Models;
using SuperSystem.Server.Services.Interfaces;
using System.Net.Http.Headers;

namespace SuperSystem.Server.Services
{
    public class SystemuserService : ISystemUser
    {
        private readonly HttpClient _client;
     
        public SystemuserService(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<CreateRequestSystemUserResponse> CreateSystemUserRequest(CreateRequestSystemUser registerSystemRequest, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("https://platform.at22.altinn.cloud/authentication/api/v1/systemuser/request/vendor", registerSystemRequest);
            return await httpResponse.Content.ReadFromJsonAsync<CreateRequestSystemUserResponse>();
    
        }
    }
}
