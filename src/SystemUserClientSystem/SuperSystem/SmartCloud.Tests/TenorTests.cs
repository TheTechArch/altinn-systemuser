using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCloud.Server.Config;
using SmartCloud.Server.Services;
using Xunit;

namespace SmartCloud.Tests;

public class TenorTests
{
    private const string Org = "123456789";
    private const string Leader = "12345678901";
    private const string Owner = "10987654321";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static HttpResponseMessage Response(object data, HttpStatusCode status = HttpStatusCode.OK) => new(status) { Content = new StringContent(JsonSerializer.Serialize(data, JsonOptions)) };
    private static object Result(params object[] documents) => new { dokumentListe = documents.Select(d => new { tenorMetadata = new { kildedata = JsonSerializer.Serialize(d, JsonOptions) } }) };
    private static object Company(params object[] groups) => new { organisasjonsnummer = Org, navn = "Testbedrift", rollegrupper = groups };
    private static object Group(string code, string id) => new { type = new { kode = code }, roller = new[] { new { type = new { kode = code }, person = new { foedselsnummer = id } } } };

    [Fact]
    public async Task Search_UsesDedicatedScopeWithoutSystemUserAndEscapesKql()
    {
        var tokens = new Tokens();
        var client = Client(request =>
        {
            Assert.Equal("testdata.api.skatteetaten.no", request.RequestUri!.Host);
            Assert.Equal("/api/testnorge/v2/soek/brreg-er-fr", request.RequestUri.AbsolutePath);
            Assert.Equal("tenor-token", request.Headers.Authorization?.Parameter);
            var query = QueryHelpers.ParseQuery(request.RequestUri.Query);
            Assert.Equal("10", query["antall"]);
            Assert.Equal("tenorMetadata", query["vis"]);
            Assert.Equal("navn:\"ACME\\\" OR navn\\:\\*\"", query["kql"]);
            return Response(Result(Company()));
        }, tokens);
        var result = await client.Search("ACME\" OR navn:*", default);
        Assert.Single(result.Organisations);
        Assert.Equal("skatteetaten:testnorge/testdata.read", tokens.LastScope);
        Assert.Null(tokens.LastOrganisation);
    }

    [Theory]
    [InlineData(true, "DAGL", Leader)]
    [InlineData(false, "INNH", Owner)]
    public async Task Details_PrefersLeaderAndUsesOwnerOnlyAsFallback(bool hasLeader, string expectedRole, string expectedId)
    {
        var tokens = new Tokens();
        var client = Client(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("brreg-er-fr"))
                return Response(Result(hasLeader ? Company(Group("INNH", Owner), Group("DAGL", Leader)) : Company(Group("INNH", Owner))));
            Assert.Equal($"foedselsnummer:{expectedId}", QueryHelpers.ParseQuery(request.RequestUri.Query)["kql"]);
            return Response(Result(new { navn = new[] { new { fornavn = "Historisk", etternavn = "Navn", erGjeldende = false }, new { fornavn = "Syntetisk", etternavn = "Testperson", erGjeldende = true } } }));
        }, tokens);
        var detail = await client.Details(Org, default);
        var person = Assert.Single(detail!.People);
        Assert.Equal(expectedRole, person.RoleCode);
        Assert.Equal(expectedId, person.NationalIdentityNumber);
        Assert.Equal("Syntetisk Testperson", person.Name);
        Assert.Equal(1, tokens.Calls);
    }

    [Fact]
    public async Task MissingRoles_DoesNotGuessPersonFromOtherNumbers()
    {
        var client = Client(_ => Response(Result(new { organisasjonsnummer = Org, navn = "Testbedrift", other = Leader })));
        Assert.Empty((await client.Details(Org, default))!.People);
    }

    [Fact]
    public async Task PersonFailure_PreservesOrganisationAndRoleWithWarning()
    {
        var client = Client(r => r.RequestUri!.AbsolutePath.EndsWith("brreg-er-fr") ? Response(Result(Company(Group("DAGL", Leader)))) : Response(new {}, HttpStatusCode.ServiceUnavailable));
        var details = await client.Details(Org, default);
        Assert.Equal(Leader, Assert.Single(details!.People).NationalIdentityNumber);
        Assert.NotNull(details.Warning);
    }

    [Theory]
    [InlineData("prod", "https://platform.tt02.altinn.no")]
    [InlineData("test", "https://platform.altinn.no")]
    public async Task NonTestConfiguration_RejectsBeforeTokenOrNetwork(string environment, string platform)
    {
        var tokens = new Tokens();
        var client = Client(_ => throw new Exception("Network must not be called"), tokens, environment, platform);
        Assert.False(client.Enabled);
        Assert.Equal(503, (await Assert.ThrowsAsync<TenorException>(() => client.Search("Test", default))).Status);
        Assert.Equal(0, tokens.Calls);
    }

    [Fact]
    public async Task InvalidSearch_RejectsBeforeToken()
    {
        var tokens = new Tokens();
        await Assert.ThrowsAsync<ValidationException>(() => Client(_ => throw new Exception(), tokens).Search(" ", default));
        Assert.Equal(0, tokens.Calls);
    }

    [Fact]
    public async Task Forbidden_ProvidesScopeAndDoesNotExposeResponseBody()
    {
        var error = await Assert.ThrowsAsync<TenorException>(() => Client(_ => Response(new { secret = "private response" }, HttpStatusCode.Forbidden)).Search("Test", default));
        Assert.Equal(403, error.UpstreamStatus);
        Assert.Contains(TenorClient.Scope, error.Message);
        Assert.DoesNotContain("private response", error.Message);
    }

    [Fact]
    public async Task MissingSourceData_ReturnsErrorInsteadOfEmptyList()
    {
        var client = Client(_ => Response(new { dokumentListe = new[] { new { tenorMetadata = new { id = Org } } } }));
        await Assert.ThrowsAsync<TenorException>(() => client.Search("Test", default));
    }

    [Theory]
    [InlineData(null, "ENK", "organisasjonsform.kode:\"ENK\"")]
    [InlineData("   ", "as", "organisasjonsform.kode:\"AS\"")]
    [InlineData("Testbedrift", "NUF", "navn:\"Testbedrift\" AND organisasjonsform.kode:\"NUF\"")]
    [InlineData("123456789", "ENK", "organisasjonsnummer:123456789 AND organisasjonsform.kode:\"ENK\"")]
    public async Task OrganisationForm_CanBeUsedAloneOrCombined(string? term, string form, string expectedQuery)
    {
        var tokens = new Tokens();
        var client = Client(request =>
        {
            Assert.Equal(expectedQuery, QueryHelpers.ParseQuery(request.RequestUri!.Query)["kql"]);
            return Response(Result(Company()));
        }, tokens);
        Assert.Single((await client.Search(term, default, form)).Organisations);
        Assert.Equal(TenorClient.Scope, tokens.LastScope);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(" ", " ")]
    [InlineData(null, "ENK OR *")]
    [InlineData(null, "AS\"")]
    [InlineData("x", "ENK")]
    public async Task InvalidFilters_FailBeforeTokenOrNetwork(string? term, string? form)
    {
        var tokens = new Tokens();
        var client = Client(_ => throw new Exception("Must not call network"), tokens);
        await Assert.ThrowsAsync<ValidationException>(() => client.Search(term, default, form));
        Assert.Equal(0, tokens.Calls);
    }
    private static TenorClient Client(Func<HttpRequestMessage, HttpResponseMessage> send, Tokens? tokens = null, string environment = "test", string platform = "https://platform.tt02.altinn.no") =>
        new(new HttpClient(new Handler(send)), tokens ?? new Tokens(), Options.Create(new MaskinportenConfig
        {
            ClientId = "", EncodedJwk = "", Environment = environment, RequestSystemUserScope = "", SystemUserScope = "", AltinnExchangeEndpoint = ""
        }), Options.Create(new SystemRegisterConfig { BaseAdress = platform, SystemId = "", SystemRegisterScope = "", RequestSystemUserScope = "", ScopeSystemUserRequestRead = "", RightResources = "", RightResourcesBasic = "", RightResourcesStandard = "" }));
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => Task.FromResult(send(request));
    }
    private sealed class Tokens : IMaskinportenService
    {
        public string? LastScope { get; private set; }
        public string? LastOrganisation { get; private set; }
        public int Calls { get; private set; }
        public Task<TokenResponse?> GetToken(string scope, string? org) { LastScope = scope; LastOrganisation = org; Calls++; return Task.FromResult<TokenResponse?>(new() { AccessToken = "tenor-token" }); }
        public Task<TokenResponse?> GetToken(JsonWebKey jwk, string environment, string clientId, string scope, string? org) => throw new NotImplementedException();
        public Task<TokenResponse?> GetToken(string jwk, string environment, string clientId, string scope, string? org) => throw new NotImplementedException();
    }
}
