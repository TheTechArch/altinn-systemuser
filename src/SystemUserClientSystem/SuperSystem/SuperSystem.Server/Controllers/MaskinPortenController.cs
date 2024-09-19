using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SuperSystem.Server.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace SuperSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaskinPortenController : ControllerBase
    {
        private readonly IMaskinportenService _maskinportenService;
        private readonly MaskinportenConfig _maskinportenConfig;

        public MaskinPortenController(IOptions<MaskinportenConfig> maskinportenConfig, IMaskinportenService maskinportenService)
        {
            _maskinportenService = maskinportenService;
            _maskinportenConfig = maskinportenConfig.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? systemUserOrg)
        {
            string? certificate = _maskinportenConfig.Cert;
            byte[] certificateBytes = Convert.FromBase64String(certificate);
            string decodedCertificate = Encoding.UTF8.GetString(certificateBytes);
            string scope = string.Empty;

            string clientId = _maskinportenConfig.ClientId;
            
            if(systemUserOrg == null)
            {
                scope = _maskinportenConfig.RequestSystemUserScope;
            }
            else
            {
                scope = _maskinportenConfig.SystemUserScope;
            }

            var rsa = RSA.Create();
            rsa.ImportFromPem(decodedCertificate.ToCharArray());

            var privRsa = new RsaSecurityKey(rsa.ExportParameters(true)) { KeyId = _maskinportenConfig.KeyId };
            var privJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(privRsa);

            HttpClient httpClient = new HttpClient();
            MaskinportenService maskinportenService = new MaskinportenService(httpClient);


            TokenResponse? systemUserToken = await _maskinportenService.GetToken(privJwk, "test", clientId, scope, systemUserOrg);
            var jwtHandler = new JwtSecurityTokenHandler();
            var decodedToken = jwtHandler.ReadJwtToken(systemUserToken.AccessToken);

            return Ok(decodedToken);
        }
    }
}
