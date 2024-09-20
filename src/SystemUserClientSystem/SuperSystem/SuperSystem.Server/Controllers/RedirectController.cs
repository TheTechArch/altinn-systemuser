using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SuperSystem.Server.Config;
using SuperSystem.Server.Models;
using SuperSystem.Server.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SuperSystem.Server.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : Controller
    {
        private readonly IMaskinportenService _maskinportenService;
        private readonly MaskinportenConfig _maskinportenConfig;
        private readonly ITokenExchange _tokenExchange;
        private readonly ISystemUser _systemUser;

        public RedirectController(IOptions<MaskinportenConfig> maskinportenConfig, IMaskinportenService maskinportenService, ITokenExchange tokenExchange, ISystemUser systemUser)
        {
            _maskinportenService = maskinportenService;
            _maskinportenConfig = maskinportenConfig.Value;
            _tokenExchange = tokenExchange;
            _systemUser = systemUser;
        }


        public async Task<IActionResult> Index([FromQuery] string? systemUserOrg)
        {
            string? certificate = _maskinportenConfig.Cert;
            byte[] certificateBytes = Convert.FromBase64String(certificate);
            string decodedCertificate = Encoding.UTF8.GetString(certificateBytes);
            string scope = string.Empty;

            string clientId = _maskinportenConfig.ClientId;

            scope = _maskinportenConfig.RequestSystemUserScope;

            var rsa = RSA.Create();
            rsa.ImportFromPem(decodedCertificate.ToCharArray());

            var privRsa = new RsaSecurityKey(rsa.ExportParameters(true)) { KeyId = _maskinportenConfig.KeyId };
            var privJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(privRsa);

            HttpClient httpClient = new HttpClient();
            MaskinportenService maskinportenService = new MaskinportenService(httpClient);

            TokenResponse? systemUserToken = await _maskinportenService.GetToken(privJwk, "test", clientId, scope, null);
            string altinntoken = await _tokenExchange.ExhangeMaskinporten(systemUserToken.AccessToken);

            CreateRequestSystemUser createSystemUserRequest = new CreateRequestSystemUser()
            {
                ExternalRef = Guid.NewGuid().ToString(),
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

            CreateRequestSystemUserResponse createRequestSystemUserResponse = await _systemUser.CreateSystemUserRequest(createSystemUserRequest, altinntoken);

            return Redirect(createRequestSystemUserResponse.ConfirmUrl);
        }
    }
}
