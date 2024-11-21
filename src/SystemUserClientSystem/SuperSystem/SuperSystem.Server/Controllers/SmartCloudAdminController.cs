using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services;
using SmartCloud.Server.Services.Interfaces;
using System.Security;

namespace smartcloud.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartCloudAdminController : ControllerBase
    {

        private readonly ISystemUser _systemUser;
        private readonly SystemRegisterConfig _systemRegisterConfig;
        private readonly IMaskinportenService _maskinporten;
        private readonly ITokenExchange _tokenExchange;
        public SmartCloudAdminController(ISystemUser systemUser, IOptions<SystemRegisterConfig> systemRegisterConfig, IMaskinportenService maskinportenService, ITokenExchange tokenExchange)
        {
            _systemUser = systemUser;
            _systemRegisterConfig = systemRegisterConfig.Value;
            _maskinporten = maskinportenService;
            _tokenExchange = tokenExchange;
        }


        [HttpGet("systemusers")]
        public async Task<IActionResult> ListAllSystemUsers()
        {
            TokenResponse token = await _maskinporten.GetToken(_systemRegisterConfig.SystemRegisterScope, null);

            string altinntoken = await _tokenExchange.ExhangeMaskinporten(token.AccessToken);
            List<SystemUser> systemUsers = await _systemUser.GetSystemUsersForSystem(_systemRegisterConfig.SystemId, altinntoken);
            return Ok(systemUsers);
        }
    }
}
