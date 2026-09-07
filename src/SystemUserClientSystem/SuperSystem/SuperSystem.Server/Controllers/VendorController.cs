using Altinn.ApiClients.Maskinporten.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services;

namespace smartcloud.server.Controllers;

[ApiController]
[Route("api/vendor")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class VendorController(AltinnVendorClient api, IMaskinportenService maskinporten, IOptions<SystemRegisterConfig> options) : ControllerBase
{
    private SystemRegisterConfig Config => options.Value;
    private static string Segment(string value) => Uri.EscapeDataString(value);
    private static string RequestPath(string kind) => kind switch
    {
        "standard" => "systemuser/request/vendor",
        "agent" => "systemuser/request/vendor/agent",
        "change" => "systemuser/changerequest/vendor",
        _ => throw new ArgumentException("Ukjent forespørselstype.")
    };
    private async Task<string> Token(string scope)
    {
        var token = await maskinporten.GetToken(scope, null);
        return !string.IsNullOrWhiteSpace(token?.AccessToken) ? token.AccessToken :
            throw new Altinn.ApiClients.Maskinporten.Models.TokenRequestException("Maskinporten returnerte ikke et tilgangstoken. Kontroller tokenresponsen fra tjenesten.");
    }

    [HttpGet("configuration")]
    public IActionResult Configuration() => Ok(new
    {
        defaultSystemId = Config.SystemId,
        environment = new Uri(Config.BaseAdress!).Host,
        defaultRedirectUrl = Config.DefaultRedirectUrl,
        presets = new[]
        {
            new { id = "basic", name = "SmartBasic", resources = Split(Config.RightResourcesBasic) },
            new { id = "standard", name = "SmartStandard", resources = Split(Config.RightResourcesStandard) }
        }
    });
    private static string[] Split(string value) => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    [HttpPost("systems")]
    public async Task<IActionResult> CreateSystem([FromBody] RegisterVendorSystem request, CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Post, "systemregister/vendor", await Token(Config.SystemRegisterScope), request, ct));

    [HttpPut("systems/{systemId}")]
    public async Task<IActionResult> UpdateSystem(string systemId, [FromBody] RegisterVendorSystem request, CancellationToken ct)
    {
        if (systemId != request.Id) return Problem(statusCode: 400, detail: "System-ID kan ikke endres.");
        return Ok(await api.Send(HttpMethod.Put, $"systemregister/vendor/{Segment(systemId)}", await Token(Config.SystemRegisterScope), request, ct));
    }
    [HttpGet("systems")]
    public async Task<IActionResult> Systems(CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Get, "systemregister/vendor", await Token(Config.SystemRegisterScope), ct: ct));

    [HttpGet("systems/{systemId}")]
    public async Task<IActionResult> SystemInfo(string systemId, CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Get, $"systemregister/vendor/{Segment(systemId)}", await Token(Config.SystemRegisterScope), ct: ct));

    [HttpGet("systems/{systemId}/users")]
    public async Task<IActionResult> Users(string systemId, CancellationToken ct) =>
        Ok(await api.List($"systemuser/vendor/bysystem/{Segment(systemId)}", await Token(Config.SystemRegisterScope), ct));

    [HttpGet("systems/{systemId}/users/lookup")]
    public async Task<IActionResult> Lookup(string systemId, [FromQuery] string orgno, [FromQuery] string? externalRef, CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Get, $"systemuser/vendor/byquery?system-id={Segment(systemId)}&orgno={Segment(orgno)}&external-ref={Segment(externalRef ?? orgno)}", await Token(Config.RequestSystemUserScope), ct: ct));

    [HttpGet("systems/{systemId}/requests/{kind:regex(^(standard|agent|change)$)}")]
    public async Task<IActionResult> Requests(string systemId, string kind, CancellationToken ct) =>
        Ok(await api.List($"{RequestPath(kind)}/bysystem/{Segment(systemId)}", await Token(Config.ScopeSystemUserRequestRead), ct));

    [HttpGet("requests/{kind:regex(^(standard|agent|change)$)}/{id:guid}")]
    public async Task<IActionResult> RequestStatus(string kind, Guid id, CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Get, $"{RequestPath(kind)}/{id}", await Token(Config.ScopeSystemUserRequestRead), ct: ct));

    [HttpPost("requests/{kind:regex(^(standard|agent)$)}")]
    public async Task<IActionResult> Create(string kind, [FromBody] VendorRequest request, CancellationToken ct)
    {
        if (kind == "agent" && (request.Rights.Count > 0 || request.AccessPackages.Count == 0))
            return Problem(statusCode: 400, detail: "Klientsystemer må bruke tilgangspakker og kan ikke ha enkeltrettigheter.");
        object body = kind == "agent"
            ? new { request.SystemId, request.PartyOrgNo, request.IntegrationTitle, request.ExternalRef, request.RedirectUrl, request.AccessPackages }
            : request;
        var result = await api.Send(HttpMethod.Post, RequestPath(kind), await Token(Config.RequestSystemUserScope), body, ct);
        return Ok(result);
    }

    [HttpPost("users/{systemUserId:guid}/change-requests/{correlationId:guid}")]
    public async Task<IActionResult> Change(Guid systemUserId, Guid correlationId, [FromBody] VendorChangeRequest request, CancellationToken ct)
    {
        if (systemUserId == Guid.Empty || correlationId == Guid.Empty || correlationId == systemUserId)
            return Problem(statusCode: 400, detail: "Bruk en unik korrelasjons-ID som er forskjellig fra systembruker-ID.");
        return Ok(await api.Send(HttpMethod.Post, $"{RequestPath("change")}?system-user-id={systemUserId}&correlation-id={correlationId}",
            await Token(Config.RequestSystemUserScope), request, ct));
    }

    // Authentication exposes vendor deletion for standard and change requests only.
    [HttpDelete("requests/{kind:regex(^(standard|change)$)}/{id:guid}")]
    public async Task<IActionResult> Delete(string kind, Guid id, CancellationToken ct) =>
        Ok(await api.Send(HttpMethod.Delete, $"{RequestPath(kind)}/{id}", await Token(Config.RequestSystemUserScope), ct: ct));
}
