using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemuserClient.Clients
{
    internal class TestApiClient
    {
        private readonly HttpClient _client;

        public TestApiClient(HttpClient httpClient) 
        {
            _client = httpClient;
        }

        public async Task<string> GetTest()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/test");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
