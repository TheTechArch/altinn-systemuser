using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Services;
using System.Text.Json;

namespace smartcloud.server.Controllers;

/// <summary>Public metadata from the same platform environment as Authentication.</summary>
[ApiController]
[Route("api/metadata")]
public class MetadataController(IHttpClientFactory factory, IOptions<SystemRegisterConfig> options, IMemoryCache cache) : ControllerBase
{
    private async Task<JsonElement> Get(string path, CancellationToken ct)
    {
        var origin = new Uri(options.Value.BaseAdress!).GetLeftPart(UriPartial.Authority);
        var url = origin + path;
        if (cache.TryGetValue<JsonElement>(url, out var cached)) return cached;
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.AcceptLanguage.ParseAdd("nb");
        using var response = await factory.CreateClient("metadata").SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode) throw new AltinnApiException(response.StatusCode, body);
        JsonElement result;
        try { using var document = JsonDocument.Parse(body); result = document.RootElement.Clone(); }
        catch (JsonException) { throw new AltinnApiException(System.Net.HttpStatusCode.BadGateway, "Metadata-API-et returnerte ugyldig JSON."); }
        cache.Set(url, result, TimeSpan.FromMinutes(10));
        return result;
    }

    [HttpGet("packages")]
    public async Task<IActionResult> Packages([FromQuery] string? term, CancellationToken ct) =>
        Ok(await Get("/accessmanagement/api/v1/meta/info/accesspackages/search?searchInResources=true&term=" + Uri.EscapeDataString(term ?? ""), ct));

    [HttpGet("packages/{id:guid}/resources")]
    public async Task<IActionResult> Resources(Guid id, CancellationToken ct) =>
        Ok(await Get($"/accessmanagement/api/v1/meta/info/accesspackages/{id}/resource", ct));

    [HttpGet("packages/by-urn")]
    public async Task<IActionResult> PackageByUrn([FromQuery] string urn, CancellationToken ct) =>
        Ok(await Get("/accessmanagement/api/v1/meta/info/accesspackages/urn/" + Uri.EscapeDataString(urn), ct));

    [HttpGet("resources")]
    public async Task<IActionResult> SearchResources([FromQuery] string? term, CancellationToken ct)
    {
        var resources = await Get("/resourceregistry/api/v1/resource/resourcelist?includeApps=true&includeAltinn2=false", ct);
        var matches = resources.EnumerateArray().Where(r => string.IsNullOrWhiteSpace(term) ||
            (r.TryGetProperty("identifier", out var id) && id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)) ||
            (r.TryGetProperty("title", out var title) && title.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
        return Ok(new { total = matches.Count, data = matches.Take(50) });
    }
    [HttpGet("resources/{id}")]
    public async Task<IActionResult> Resource(string id, CancellationToken ct) =>
        Ok(await Get("/resourceregistry/api/v1/resource/" + Uri.EscapeDataString(id), ct));
}
