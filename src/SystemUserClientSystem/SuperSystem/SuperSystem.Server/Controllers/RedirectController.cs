using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SmartCloud.Server.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : Controller
    {
        private readonly IMaskinportenService _maskinportenService;
        private readonly MaskinportenConfig _maskinportenConfig;
        private readonly ITokenExchange _tokenExchange;
        private readonly ISystemUser _systemUser;
        private readonly SystemRegisterConfig _systemRegisterConfig;

        public RedirectController(IOptions<MaskinportenConfig> maskinportenConfig, IMaskinportenService maskinportenService, ITokenExchange tokenExchange, ISystemUser systemUser, IOptions<SystemRegisterConfig> systemRegisterConfig)
        {
            _maskinportenService = maskinportenService;
            _maskinportenConfig = maskinportenConfig.Value;
            _tokenExchange = tokenExchange;
            _systemUser = systemUser;
            _systemRegisterConfig = systemRegisterConfig.Value;
        }


        public async Task<IActionResult> Index([FromQuery] string systemUserOrg)
        {
            string scope = _systemRegisterConfig.RequestSystemUserScope;

            TokenResponse? systemUserToken = await _maskinportenService.GetToken(scope, null);
            string altinntoken = await _tokenExchange.ExhangeMaskinporten(systemUserToken.AccessToken);

            CreateRequestSystemUser createSystemUserRequest = new CreateRequestSystemUser()
            {
                ExternalRef = Guid.NewGuid().ToString(),
                PartyOrgNo = systemUserOrg,
                SystemId = _systemRegisterConfig.SystemId,
                Rights = new List<Right>(),
                RedirectUrl = "https://smartcloudaltinn.azurewebsites.net/receipt"
            };

            string[] resources = _systemRegisterConfig.RightResources.Split(",");

            foreach(string resource in resources){
                
                createSystemUserRequest.Rights.Add(new Right()
                {
                    Resource = [new AttributePair() { Id = "urn:altinn:resource", Value = resource }]
                });
            
            }


            CreateRequestSystemUserResponse createRequestSystemUserResponse = await _systemUser.CreateSystemUserRequest(createSystemUserRequest, altinntoken);

            return Redirect(createRequestSystemUserResponse.ConfirmUrl);
        }
    }
}
