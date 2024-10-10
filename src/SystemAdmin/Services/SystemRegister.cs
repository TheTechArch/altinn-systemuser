using Altinn.Platform.Authentication.Core.SystemRegister.Models;
using Altinn.SystemAdmin.Maskinporten.Config;
using Altinn.SystemAdmin.Maskinporten.Interfaces;
using Altinn.SystemAdmin.Maskinporten.Models;
using Altinn.SystemAdmin.Maskinporten.Services;
using SuperSystem.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SystemAdmin.Config;
using SystemAdmin.Services.Interfaces;

namespace SystemAdmin.Services
{
    internal class SystemRegister: ISystemRegister
    {
        private readonly HttpClient _httpClient;
        private readonly IMaskinportenService _maskinportenService;
        private readonly SystemRegisterConfig _systemRegisterConfig;
       
        public SystemRegister(HttpClient httpClient, IMaskinportenService maskinportenService, SystemRegisterConfig systemRegisterConfig)
        {
            _httpClient = httpClient;
            _maskinportenService = maskinportenService;
            _systemRegisterConfig = systemRegisterConfig;
         }

        public async Task CreateSystem(SystemRegisterRequest request)
        {
            TokenResponse tokenResponse = await _maskinportenService.GetToken(_systemRegisterConfig.SystemRegisterScope , null);

            TokenExchange tokenExchange = new TokenExchange(new HttpClient());

            string exchangedToken = tokenExchange.ExhangeMaskinporten(tokenResponse.AccessToken).Result;

            _httpClient.BaseAddress = new Uri(_systemRegisterConfig.SystemRegisterBaseAdress);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {exchangedToken}");

            //HttpResponseMessage response = await _httpClient.PostAsJsonAsync("systemregister/vendor", request);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync("systemregister/vendor/991825827_smartcloud", request);
          
            string responseText = await response.Content.ReadAsStringAsync();
        }

    }
   
}
