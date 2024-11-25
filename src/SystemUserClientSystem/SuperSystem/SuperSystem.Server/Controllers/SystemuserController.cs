using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using smartcloud.server.Models;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;

namespace smartcloud.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemuserController : ControllerBase
    {
        private readonly ISystemUser _systemUser;
        private readonly IMaskinportenService _maskinportenService;
        private readonly ITokenExchange _tokenExchange;
        private readonly SystemRegisterConfig _systemRegisterConfig;
        private readonly MaskinportenConfig _maskinportenConfig;

        public SystemuserController(IOptions<MaskinportenConfig> maskinportenConfig, IMaskinportenService maskinportenService, ITokenExchange tokenExchange, ISystemUser systemUser, IOptions<SystemRegisterConfig> systemRegisterConfig)
        {
            _systemUser = systemUser;
            _maskinportenService = maskinportenService;
            _tokenExchange = tokenExchange;
            _systemRegisterConfig = systemRegisterConfig.Value;
            _maskinportenConfig = maskinportenConfig.Value;
        }


        [HttpGet("list")]
        public async Task<List<SystemUser>> List()
        {
            string scope = _systemRegisterConfig.SystemRegisterScope;

            TokenResponse? systemUserToken = await _maskinportenService.GetToken(scope, null);

            return await _systemUser.GetSystemUsersForSystem(_systemRegisterConfig.SystemId, systemUserToken.AccessToken);
        }

        [HttpGet("requests")]
        public async Task<List<RequestSystemResponse>> Requests()
        {
            string scope = _systemRegisterConfig.ScopeSystemUserRequestRead;

            TokenResponse? systemUserToken = await _maskinportenService.GetToken(scope, null);

            return await _systemUser.GetRequestsForSystem(_systemRegisterConfig.SystemId, systemUserToken.AccessToken);
        }
        

    }
}
