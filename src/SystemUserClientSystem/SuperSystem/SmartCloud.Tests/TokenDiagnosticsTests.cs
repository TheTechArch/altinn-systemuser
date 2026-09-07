using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCloud.Server.Config;
using SmartCloud.Server.Filters;
using Xunit;

namespace SmartCloud.Tests;

public class TokenDiagnosticsTests
{
    private const string Scope = "altinn:authentication/systemregister.write";

    [Theory]
    [InlineData("invalid_scope", "MP-200", "scopet")]
    [InlineData("invalid_grant", "MP-121", "utløpt")]
    [InlineData("invalid_client", null, "ClientId")]
    public async Task TokenFailure_ExposesActionableDiagnosticsWithoutRawProviderDescription(string code, string? providerCode, string advice)
    {
        var body = JsonSerializer.Serialize(new { error = code, error_description = $"{providerCode}: sensitive-assertion-must-not-be-returned" });
        var error = await Fail(body, HttpStatusCode.BadRequest);
        Assert.Equal(code, error.Code);
        Assert.Equal(providerCode, error.ProviderCode);
        Assert.Contains(advice, error.Message);
        var problem = Filter(error);
        Assert.Equal(502, problem.Status);
        Assert.Equal(400, problem.Extensions["upstreamStatus"]);
        Assert.Equal(Scope, problem.Extensions["scope"]);
        Assert.Equal("test", problem.Extensions["environment"]);
        Assert.Equal("Maskinporten", problem.Extensions["service"]);
        Assert.Equal("test-trace", problem.Extensions["traceId"]);
        Assert.DoesNotContain("sensitive-assertion", JsonSerializer.Serialize(problem));
    }

    [Theory]
    [InlineData("<html>proxy failure secret</html>", 502)]
    [InlineData("{}", 200)]
    [InlineData("null", 200)]
    [InlineData("not-json", 200)]
    public async Task MalformedResponse_ReturnsHandledDiagnostic(string body, int status)
    {
        var error = await Fail(body, (HttpStatusCode)status);
        Assert.Equal("invalid_response", error.Code);
        Assert.Equal(502, Filter(error).Status);
        Assert.DoesNotContain("secret", error.Message);
    }

    [Theory]
    [InlineData(false, "connection_failed")]
    [InlineData(true, "timeout")]
    public async Task TransportFailure_IdentifiesMaskinporten(bool timeout, string code)
    {
        var service = Service(_ => throw (timeout ? new TaskCanceledException("private details") : new HttpRequestException("private details")));
        var error = await Invoke(service);
        Assert.Equal(code, error.Code);
        Assert.Null(error.UpstreamStatus);
        Assert.DoesNotContain("private details", JsonSerializer.Serialize(Filter(error)));
    }

    [Fact]
    public void AltinnForbiddenWithoutProblemBody_ExplainsAccessFailure()
    {
        var problem = Filter(new SmartCloud.Server.Services.AltinnApiException(HttpStatusCode.Forbidden, "<html>secret</html>"));
        Assert.Equal(403, problem.Status);
        Assert.Equal("Altinn", problem.Extensions["service"]);
        Assert.Contains("scope", problem.Detail);
        Assert.DoesNotContain("secret", JsonSerializer.Serialize(problem));
    }

    [Fact]
    public async Task SuccessfulTokenResponse_RemainsUsable()
    {
        var service = Service(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"access_token\":\"test-token\",\"token_type\":\"Bearer\"}") }));
        using var rsa = RSA.Create(2048);
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa));
        var token = await service.GetToken(jwk, "test", "test-client", Scope, null);
        Assert.Equal("test-token", token?.AccessToken);
    }

    [Fact]
    public void MalformedAltinnResponse_ExplainsParsingFailure()
    {
        var problem = Filter(SmartCloud.Server.Services.AltinnApiException.InvalidResponse("Altinn returnerte et ugyldig listesvar."));
        Assert.Equal(502, problem.Status);
        Assert.Contains("ugyldig listesvar", problem.Detail);
    }
    private static Task<TokenRequestException> Fail(string body, HttpStatusCode status) =>
        Invoke(Service(_ => Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body) })));

    private static async Task<TokenRequestException> Invoke(MaskinportenService service)
    {
        using var rsa = RSA.Create(2048);
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa));
        return await Assert.ThrowsAsync<TokenRequestException>(() => service.GetToken(jwk, "test", "test-client", Scope, null));
    }

    private static MaskinportenService Service(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) =>
        new(new HttpClient(new Handler(send)), Options.Create(new MaskinportenConfig { ClientId = "test-client", RequestSystemUserScope = "", SystemUserScope = "", EncodedJwk = "", AltinnExchangeEndpoint = "", Environment = "test" }));

    private static ProblemDetails Filter(Exception exception)
    {
        var http = new DefaultHttpContext { TraceIdentifier = "test-trace" };
        var context = new ExceptionContext(new ActionContext(http, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>()) { Exception = exception };
        new AltinnExceptionFilter(NullLogger<AltinnExceptionFilter>.Instance).OnException(context);
        Assert.True(context.ExceptionHandled);
        return Assert.IsType<ProblemDetails>(Assert.IsType<ObjectResult>(context.Result).Value);
    }

    private class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request);
    }
}
