using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;

namespace SmartCloud.Server.Services;

/// <summary>HTTP transport for the vendor APIs. Never follows next-link hosts with a bearer token.</summary>
public class AltinnVendorClient(HttpClient client, IOptions<SystemRegisterConfig> options)
{
    private Uri BaseUri => new(options.Value.BaseAdress!.TrimEnd('/') + "/");

    public async Task<JsonElement> Send(HttpMethod method, string path, string? token, object? body = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(method, new Uri(BaseUri, path));
        if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await client.SendAsync(request, ct);
        var text = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode) throw new AltinnApiException(response.StatusCode, text);
        if (string.IsNullOrWhiteSpace(text)) return JsonSerializer.SerializeToElement<object?>(null);
        try { using var document = JsonDocument.Parse(text); return document.RootElement.Clone(); }
        catch (JsonException) { throw AltinnApiException.InvalidResponse("Altinn returnerte et ugyldig JSON-svar."); }
    }

    public async Task<List<JsonElement>> List(string path, string token, CancellationToken ct = default)
    {
        var items = new List<JsonElement>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var nextPath = path;
        for (var page = 0; page < 1000; page++)
        {
            var result = await Send(HttpMethod.Get, nextPath, token, ct: ct);
            if (result.ValueKind != JsonValueKind.Object || !result.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
                throw AltinnApiException.InvalidResponse("Altinn returnerte et ugyldig listesvar.");
            items.AddRange(data.EnumerateArray());
            if (!result.TryGetProperty("links", out var links) || !links.TryGetProperty("next", out var next) ||
                next.ValueKind == JsonValueKind.Null || string.IsNullOrEmpty(next.GetString())) return items;
            // Altinn can return an internal host in links.next. Only reuse its opaque token on the original endpoint.
            if (!Uri.TryCreate(BaseUri, next.GetString(), out var uri) ||
                !QueryHelpers.ParseQuery(uri.Query).TryGetValue("token", out var continuation) ||
                continuation.Count != 1 || string.IsNullOrEmpty(continuation[0]) || !seen.Add(continuation[0]!))
                throw AltinnApiException.InvalidResponse("Altinn returnerte en ugyldig eller gjentatt fortsettelsesmarkør.");
            nextPath = QueryHelpers.AddQueryString(path, "token", continuation[0]!);
        }
        throw AltinnApiException.InvalidResponse("For mange sider fra Altinn. Listen ble ikke fullført.");
    }
}

public class AltinnApiException(HttpStatusCode status, string responseBody, string? safeDetail = null) : Exception("Altinn API-kallet feilet.")
{
    public string? SafeDetail { get; } = safeDetail;
    public static AltinnApiException InvalidResponse(string safeDetail) => new(HttpStatusCode.BadGateway, "", safeDetail);
    public int StatusCode { get; } = (int)status;
    public string ResponseBody { get; } = responseBody;
}
