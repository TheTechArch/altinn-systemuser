using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace SmartCloud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaskinPortenController : ControllerBase
    {
        private readonly IMaskinportenService _maskinportenService;
        private readonly MaskinportenConfig _maskinportenConfig;
        private readonly ITokenExchange _tokenExchange;
        private readonly ISystemUser _systemUser;

        public MaskinPortenController(IOptions<MaskinportenConfig> maskinportenConfig, IMaskinportenService maskinportenService, ITokenExchange tokenExchange, ISystemUser systemUser)
        {
            _maskinportenService = maskinportenService;
            _maskinportenConfig = maskinportenConfig.Value;
            _tokenExchange = tokenExchange;
            _systemUser = systemUser;
        }

        [HttpGet("createsystemuserrequest")]
        public async Task<IActionResult> CreateSystemUserRequest([FromQuery] string systemUserOrg)
        {
            TokenResponse? systemUserToken = await _maskinportenService.GetToken(_maskinportenConfig.RequestSystemUserScope, null);

            string altinntoken = await _tokenExchange.ExhangeMaskinporten(systemUserToken.AccessToken);

            CreateRequestSystemUser createSystemUserRequest = new CreateRequestSystemUser()
            {  ExternalRef = Guid.NewGuid().ToString(),
                PartyOrgNo = systemUserOrg,
                SystemId = "312600757_smartcloud",
                Rights = new List<Right>()
                {
                    new Right()
                    {
                        Resource = [new AttributePair() { Id = "urn:altinn:resource", Value = "kravogbetaling" }]
                    }
                },
                RedirectUrl = "https://smartcloudaltinn.azurewebsites.net/receipt"
            };

            CreateRequestSystemUserResponse createRequestSystemUserResponse = await  _systemUser.CreateSystemUserRequest(createSystemUserRequest, altinntoken);

            return Ok(createRequestSystemUserResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? systemUserOrg)
        {
          
            string scope = string.Empty;

            string clientId = _maskinportenConfig.ClientId;

            if (systemUserOrg == null)
            {
                scope = _maskinportenConfig.RequestSystemUserScope;
            }
            else
            {
                scope = _maskinportenConfig.SystemUserScope;
            }

            TokenResponse? systemUserToken = await _maskinportenService.GetToken(scope, systemUserOrg);
            string altinntoken = await _tokenExchange.ExhangeMaskinporten(systemUserToken.AccessToken);
            // var jwtHandler = new JwtSecurityTokenHandler();
            // var decodedToken = jwtHandler.ReadJwtToken(systemUserToken.AccessToken);

            return Ok(altinntoken);
        }
    }
}
