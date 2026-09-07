using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using smartcloud.server.Controllers;
using SmartCloud.Server.Config;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services;
using Xunit;

namespace SmartCloud.Tests;

public class VendorContractTests
{
    private static SystemRegisterConfig Config => new()
    {
        BaseAdress = "https://platform.tt02.altinn.no/authentication/api/v1/",
        SystemId = "991825827_smartcloud", SystemRegisterScope = "register.write",
        RequestSystemUserScope = "request.write", ScopeSystemUserRequestRead = "request.read",
        RightResources = "", RightResourcesBasic = "", RightResourcesStandard = ""
    };
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static HttpResponseMessage Json(string body, HttpStatusCode status = HttpStatusCode.OK) =>
        new(status) { Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json") };
    private static AltinnVendorClient Client(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) =>
        new(new HttpClient(new Handler(send)), Options.Create(Config));

    [Fact]
    public async Task Pagination_UsesOpaqueTokenOnOriginalEndpoint_EvenWithInternalNextHost()
    {
        var calls = new List<string>();
        var client = Client(request =>
        {
            calls.Add(request.RequestUri!.AbsoluteUri);
            Assert.Equal("vendor-token", request.Headers.Authorization?.Parameter);
            return Task.FromResult(calls.Count == 1
                ? Json("""{"links":{"next":"http://internal.altinn.cloud/anything?token=abc%2B%2F%3D"},"data":[{"id":"one"}]}""")
                : Json("""{"links":{"next":null},"data":[{"id":"two"}]}"""));
        });
        var result = await client.List("systemuser/vendor/bysystem/991825827_smartcloud", "vendor-token");
        Assert.Equal(2, result.Count);
        Assert.All(calls, url => Assert.StartsWith(Config.BaseAdress!, url));
        Assert.EndsWith("?token=abc%2B%2F%3D", calls[1]);
    }

    [Fact]
    public async Task Pagination_RejectsRepeatedTokenInsteadOfHangingOrReturningPartialList()
    {
        var client = Client(_ => Task.FromResult(Json("""{"links":{"next":"?token=again"},"data":[]}""")));
        var exception = await Assert.ThrowsAsync<AltinnApiException>(() => client.List("systemuser/request/vendor/bysystem/example", "token"));
        Assert.Equal(502, exception.StatusCode);
    }

    [Fact]
    public async Task Pagination_RejectsMalformedResponse()
    {
        var client = Client(_ => Task.FromResult(Json("""{"unexpected":[]}""")));
        await Assert.ThrowsAsync<AltinnApiException>(() => client.List("systemuser/vendor/bysystem/example", "token"));
    }

    [Fact]
    public async Task UpstreamProblem_IsPreserved()
    {
        var client = Client(_ => Task.FromResult(Json("""{"detail":"Access package is not registered","code":"AUTH-001"}""", HttpStatusCode.BadRequest)));
        var exception = await Assert.ThrowsAsync<AltinnApiException>(() => client.Send(HttpMethod.Post, "systemuser/request/vendor", "token", new {}));
        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("AUTH-001", exception.ResponseBody);
    }

    [Theory]
    [InlineData("standard", "systemuser/request/vendor")]
    [InlineData("agent", "systemuser/request/vendor/agent")]
    public async Task Create_UsesCorrectEndpointScopeAndPackageContract(string kind, string endpoint)
    {
        JsonElement sent = default;
        var tokens = new Tokens();
        var controller = new VendorController(Client(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith(endpoint, request.RequestUri!.AbsoluteUri);
            sent = JsonSerializer.Deserialize<JsonElement>(await request.Content!.ReadAsStringAsync());
            return Json("""{"id":"590a8e76-6908-41ef-96bc-3fc8c919935b","status":"New"}""");
        }), tokens, Options.Create(Config));
        await controller.Create(kind, new VendorRequest
        {
            SystemId = Config.SystemId, PartyOrgNo = "123456789", IntegrationTitle = "Payroll",
            ExternalRef = "tenant-1", AccessPackages = [new() { Urn = "urn:altinn:accesspackage:skattegrunnlag" }]
        }, default);
        Assert.Equal("request.write", tokens.LastScope);
        Assert.Equal("Payroll", sent.GetProperty("integrationTitle").GetString());
        Assert.Equal("tenant-1", sent.GetProperty("externalRef").GetString());
        Assert.Equal("urn:altinn:accesspackage:skattegrunnlag", sent.GetProperty("accessPackages")[0].GetProperty("urn").GetString());
        Assert.Equal(kind == "standard", sent.TryGetProperty("rights", out _));
    }

    [Fact]
    public async Task Agent_RejectsIndividualRightsBeforeCallingAltinn()
    {
        var controller = new VendorController(Client(_ => throw new Exception("Must not call Altinn")), new Tokens(), Options.Create(Config))
        { ControllerContext = new() { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() } };
        var result = await controller.Create("agent", new VendorRequest
        {
            Rights = [new() { Resource = [new() { Id = "urn:altinn:resource", Value = "demo" }] }]
        }, default);
        Assert.Equal(400, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task RegisterSystem_PreservesVendorIdCasingAndFullConfiguration()
    {
        var tokens = new Tokens();
        var controller = new VendorController(Client(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("systemregister/vendor/991825827_smartcloud", request.RequestUri!.AbsoluteUri);
            var body = JsonSerializer.Deserialize<JsonElement>(await request.Content!.ReadAsStringAsync());
            Assert.Equal("0192:991825827", body.GetProperty("vendor").GetProperty("ID").GetString());
            Assert.Equal("Name", body.GetProperty("name").GetProperty("en").GetString());
            Assert.Equal("client-1", body.GetProperty("clientId")[0].GetString());
            Assert.Equal("https://example.test/receipt", body.GetProperty("allowedRedirectUrls")[0].GetString());
            Assert.Single(body.GetProperty("accessPackages").EnumerateArray());
            return Json("""{"success":true}""");
        }), tokens, Options.Create(Config));
        await controller.UpdateSystem(Config.SystemId, new RegisterVendorSystem
        {
            Id = Config.SystemId, Vendor = new() { Id = "0192:991825827" },
            Name = new() { ["nb"] = "Navn", ["en"] = "Name" }, Description = new() { ["en"] = "Description" },
            ClientId = ["client-1"], AllowedRedirectUrls = ["https://example.test/receipt"],
            AccessPackages = [new() { Urn = "urn:altinn:accesspackage:skattegrunnlag" }]
        }, default);
        Assert.Equal("register.write", tokens.LastScope);
    }

    [Fact]
    public async Task ChangeRequest_UsesSeparateCorrelationAndSystemUserIds()
    {
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var controller = new VendorController(Client(async request =>
        {
            Assert.Contains($"system-user-id={userId}&correlation-id={correlationId}", request.RequestUri!.Query);
            var body = JsonSerializer.Deserialize<JsonElement>(await request.Content!.ReadAsStringAsync());
            Assert.Single(body.GetProperty("unwantedAccessPackages").EnumerateArray());
            return Json("""{"status":"New"}""");
        }), new Tokens(), Options.Create(Config));
        await controller.Change(userId, correlationId, new VendorChangeRequest
        {
            UnwantedAccessPackages = [new() { Urn = "urn:altinn:accesspackage:skattegrunnlag" }]
        }, default);
    }

    [Fact]
    public void PackageOnlyRequest_IsValid_EmptyAndMalformedAreNot()
    {
        var request = new VendorRequest { SystemId = Config.SystemId, PartyOrgNo = "123456789", AccessPackages = [new() { Urn = "urn:altinn:accesspackage:demo" }] };
        Assert.Empty(Validate(request));
        request.AccessPackages.Clear();
        Assert.NotEmpty(Validate(request));
        request.Rights = [new() { Resource = [] }];
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void ChangeRequest_RejectsOverlappingPackagesAndHandlesNullResource()
    {
        var request = new VendorChangeRequest
        {
            RequiredAccessPackages = [new() { Urn = "urn:altinn:accesspackage:demo" }],
            UnwantedAccessPackages = [new() { Urn = "urn:altinn:accesspackage:demo" }],
            RequiredRights = [new() { Resource = null! }]
        };
        Assert.True(Validate(request).Count >= 2);
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("http://external.example/receipt")]
    [InlineData("/receipt")]
    public void InvalidRedirect_IsRejected(string redirect)
    {
        Assert.False(RequestValidation.ValidRedirect(redirect));
    }

    [Fact]
    public void WrongVendorPrefix_IsRejected()
    {
        Assert.NotEmpty(Validate(new RegisterVendorSystem
        {
            Id = "123456789_system", Vendor = new() { Id = "0192:991825827" }, Name = new() { ["nb"] = "Demo" }
        }));
    }

    [Fact]
    public async Task MissingMaskinportenConfiguration_FailsBeforeNetworkCall()
    {
        var config = new MaskinportenConfig
        {
            EncodedJwk = "", ClientId = "", RequestSystemUserScope = "", SystemUserScope = "",
            AltinnExchangeEndpoint = "", Environment = "test"
        };
        var service = new Altinn.ApiClients.Maskinporten.Services.MaskinportenService(
            new HttpClient(new Handler(_ => throw new Exception("Must not call the network"))), Options.Create(config));
        var exception = await Assert.ThrowsAsync<MaskinportenConfigurationException>(() => service.GetToken("scope", null));
        Assert.Contains("Maskinporten:EncodedJwk", exception.Message);
    }
    private static List<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, true);
        return results;
    }
    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => send(request);
    }
    private sealed class Tokens : IMaskinportenService
    {
        public string? LastScope { get; private set; }
        public Task<TokenResponse?> GetToken(string scope, string? org) { LastScope = scope; return Task.FromResult<TokenResponse?>(new() { AccessToken = "vendor-token" }); }
        public Task<TokenResponse?> GetToken(JsonWebKey jwk, string environment, string clientId, string scope, string? org) => throw new NotImplementedException();
        public Task<TokenResponse?> GetToken(string jwk, string environment, string clientId, string scope, string? org) => throw new NotImplementedException();
    }
}
