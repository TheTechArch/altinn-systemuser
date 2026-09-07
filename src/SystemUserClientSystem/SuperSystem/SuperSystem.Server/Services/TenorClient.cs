using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;

namespace SmartCloud.Server.Services;

public record TenorOrganisation(string OrganisationNumber, string Name);
public record TenorPerson(string NationalIdentityNumber, string? Name, string RoleCode, string RoleName);
public record TenorOrganisationDetails(string OrganisationNumber, string Name, List<TenorPerson> People, string? Warning);
public record TenorSearchResult(List<TenorOrganisation> Organisations, bool HasMore);
public class TenorException(string message, int status = 502, int? upstreamStatus = null) : Exception(message)
{
    public int Status { get; } = status;
    public int? UpstreamStatus { get; } = upstreamStatus;
}

/// <summary>Read-only, bounded searches of synthetic test data. Never exchanges the token with Altinn.</summary>
public class TenorClient(HttpClient client, IMaskinportenService tokens, IOptions<MaskinportenConfig> maskinporten, IOptions<SystemRegisterConfig> system)
{
    public const string Scope = "skatteetaten:testnorge/testdata.read";
    public const string BaseUrl = "https://testdata.api.skatteetaten.no/api/testnorge/v2/soek/";
    public bool Enabled => maskinporten.Value.Environment == "test" &&
        Uri.TryCreate(system.Value.BaseAdress, UriKind.Absolute, out var uri) && uri.Host == "platform.tt02.altinn.no";

    private async Task<string> Token()
    {
        if (!Enabled) throw new TenorException("Tenor-søk er bare tilgjengelig med Altinn TT02 og Maskinporten:Environment=test.", 503);
        var token = await tokens.GetToken(Scope, null);
        return !string.IsNullOrWhiteSpace(token?.AccessToken) ? token.AccessToken :
            throw new TokenRequestException("invalid_response", "test", Scope);
    }

    public async Task<TenorSearchResult> Search(string term, CancellationToken ct)
    {
        term = term.Trim();
        if (term.Length is < 2 or > 100 || term.Any(char.IsControl)) throw new ValidationException("Skriv 2–100 tegn i virksomhetsnavnet eller et organisasjonsnummer.");
        // Keep user input a literal KQL value, never a query supplied by the browser.
        var kql = Regex.IsMatch(term, @"^[0-9]{9}$") ? $"organisasjonsnummer:{term}" : $"navn:{Quote(term)}";
        var result = await SearchSource("brreg-er-fr", kql, 10, await Token(), ct);
        return new(result.Documents.Select(Organisation).ToList(), result.HasMore);
    }

    public async Task<TenorOrganisationDetails?> Details(string organisationNumber, CancellationToken ct)
    {
        if (!Regex.IsMatch(organisationNumber, @"^[0-9]{9}$")) throw new ValidationException("Organisasjonsnummer må ha 9 siffer.");
        var token = await Token();
        var result = await SearchSource("brreg-er-fr", $"organisasjonsnummer:{organisationNumber}", 1, token, ct);
        if (result.Documents.Count == 0) return null;
        var data = result.Documents[0];
        var org = Organisation(data);
        if (org.OrganisationNumber != organisationNumber) throw InvalidResponse();
        var people = new List<TenorPerson>();
        foreach (var group in Array(data, "rollegrupper"))
        foreach (var role in Array(group, "roller"))
        {
            var code = Text(Object(role, "type"), "kode") ?? Text(Object(group, "type"), "kode");
            if (code is not ("DAGL" or "INNH")) continue;
            var id = Text(Object(role, "person"), "foedselsnummer");
            if (id is null || !Regex.IsMatch(id, @"^[0-9]{11}$")) continue;
            people.Add(new(id, null, code, code == "DAGL" ? "Daglig leder" : "Innehaver"));
        }
        // Prefer actual DAGL roles; only use INNH when none exist.
        var preferredRole = people.Any(p => p.RoleCode == "DAGL") ? "DAGL" : "INNH";
        people = people.Where(p => p.RoleCode == preferredRole)
            .DistinctBy(p => p.NationalIdentityNumber).Take(10).ToList();
        string? warning = null;
        for (var i = 0; i < people.Count; i++)
        {
            try
            {
                var person = await SearchSource("freg", $"foedselsnummer:{people[i].NationalIdentityNumber}", 1, token, ct);
                var names = person.Documents.Count == 0 ? [] : Array(person.Documents[0], "navn");
                var current = names.FirstOrDefault(n => Object(n, "erGjeldende").ValueKind == JsonValueKind.True);
                var name = current.ValueKind == JsonValueKind.Undefined ? null : string.Join(" ", new[] { Text(current, "fornavn"), Text(current, "mellomnavn"), Text(current, "etternavn") }.Where(n => !string.IsNullOrWhiteSpace(n)));
                people[i] = people[i] with { Name = string.IsNullOrWhiteSpace(name) ? null : name };
            }
            catch (TenorException)
            {
                warning = "Virksomhet og rolle er hentet, men personnavn kunne ikke hentes fra Tenor. Du kan bruke det syntetiske fødselsnummeret.";
            }
        }
        return new(org.OrganisationNumber, org.Name, people, warning);
    }

    private static string Quote(string value) => "\"" + Regex.Replace(value, @"[\\\""():*]", @"\$0") + "\"";
    private static TenorOrganisation Organisation(JsonElement data)
    {
        var id = Text(data, "organisasjonsnummer");
        var name = Text(data, "navn");
        if (id is null || !Regex.IsMatch(id, @"^[0-9]{9}$") || string.IsNullOrWhiteSpace(name)) throw InvalidResponse();
        return new(id, name);
    }
    private static JsonElement Object(JsonElement data, string key) => data.ValueKind == JsonValueKind.Object && data.TryGetProperty(key, out var value) ? value : default;
    private static string? Text(JsonElement data, string key) => Object(data, key) is var value && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    private static List<JsonElement> Array(JsonElement data, string key) => Object(data, key) is var value && value.ValueKind == JsonValueKind.Array ? value.EnumerateArray().ToList() : [];
    private static TenorException InvalidResponse() => new("Tenor returnerte ugyldige eller manglende kildedata. Prøv igjen eller kontroller kilden i Tenor.");

    private async Task<(List<JsonElement> Documents, bool HasMore)> SearchSource(string source, string kql, int count, string token, CancellationToken ct)
    {
        var url = QueryHelpers.AddQueryString(BaseUrl + source, new Dictionary<string, string?> { ["kql"] = kql, ["vis"] = "tenorMetadata", ["antall"] = count.ToString() });
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.ParseAdd("application/json");
        try
        {
            using var response = await client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
                throw new TenorException((int)response.StatusCode switch
                {
                    401 or 403 => $"Tenor avviste tilgangen. Kontroller at virksomheten og Maskinporten-klienten har tilgang til {Scope} i test.",
                    429 => "Tenor begrenser antall søk. Vent litt og prøv igjen.",
                    _ => "Tenor-søket feilet. Prøv igjen og kontroller tjenestens driftsstatus."
                }, upstreamStatus: (int)response.StatusCode);
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            if (Object(document.RootElement, "dokumentListe").ValueKind != JsonValueKind.Array) throw InvalidResponse();
            var documents = new List<JsonElement>();
            foreach (var item in Array(document.RootElement, "dokumentListe").Take(count))
            {
                var raw = Text(Object(item, "tenorMetadata"), "kildedata");
                if (raw is null) throw InvalidResponse();
                using var data = JsonDocument.Parse(raw);
                if (data.RootElement.ValueKind != JsonValueKind.Object) throw InvalidResponse();
                documents.Add(data.RootElement.Clone());
            }
            return (documents, Object(document.RootElement, "nesteSide").ValueKind == JsonValueKind.Number);
        }
        catch (JsonException) { throw InvalidResponse(); }
        catch (HttpRequestException) { throw new TenorException("SmartCloud kunne ikke koble til Tenor. Kontroller nettverkstilgangen fra serveren."); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) { throw new TenorException("Tenor svarte ikke innen tidsfristen. Prøv igjen."); }
    }
}
