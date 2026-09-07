using SmartCloud.Server.Models;
using Altinn.ApiClients.Maskinporten.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartCloud.Server.Services;
using System.Text.Json;

namespace SmartCloud.Server.Filters;

public class AltinnExceptionFilter(ILogger<AltinnExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is not (AltinnApiException or HttpRequestException or TokenRequestException or TaskCanceledException or MaskinportenConfigurationException))
            return;
        if (context.HttpContext.RequestAborted.IsCancellationRequested) return;
        var status = exception is MaskinportenConfigurationException ? 503 : exception is AltinnApiException apiError ? apiError.StatusCode : 502;
        var problem = new ProblemDetails
        {
            Status = status,
            Title = "Kallet til Altinn eller Maskinporten feilet",
            Detail = "Kontroller konfigurasjon, tilganger og forespørselen, og prøv igjen.",
        };
        if (exception is MaskinportenConfigurationException)
        {
            problem.Title = "Maskinporten er ikke konfigurert";
            problem.Detail = exception.Message;
            problem.Extensions["service"] = "Maskinporten";
        }
        if (exception is TokenRequestException token)
        {
            problem.Title = "Kunne ikke hente tilgangstoken fra Maskinporten";
            problem.Detail = token.Message;
            problem.Extensions["service"] = "Maskinporten";
            problem.Extensions["code"] = token.Code;
            problem.Extensions["providerCode"] = token.ProviderCode;
            problem.Extensions["environment"] = token.Environment;
            problem.Extensions["scope"] = token.Scope;
            problem.Extensions["upstreamStatus"] = token.UpstreamStatus;
        }
        if (exception is HttpRequestException or TaskCanceledException)
        {
            problem.Title = "Kunne ikke nå Altinn";
            problem.Extensions["service"] = "Altinn";
        }
        if (exception is HttpRequestException)
            problem.Detail = "SmartCloud kunne ikke koble til Altinn. Kontroller DNS, TLS og utgående nettverkstilgang fra serveren.";
        if (exception is TaskCanceledException)
            problem.Detail = "Kallet til Altinn overskred tidsfristen. Prøv igjen og kontroller tjenestens driftsstatus.";
        if (exception is AltinnApiException altinn)
        {
            problem.Title = "Kallet til Altinn feilet";
            problem.Extensions["service"] = "Altinn";
            if (altinn.SafeDetail is null) problem.Extensions["upstreamStatus"] = altinn.StatusCode;
            problem.Detail = altinn.SafeDetail ?? (altinn.StatusCode switch
            {
                401 => "Altinn avviste tilgangstokenet. Kontroller at Maskinporten og Altinn bruker riktig miljø.",
                403 => "Altinn avviste tilgangen. Kontroller tokenets scope og virksomhetens tilgang til dette API-et.",
                404 => "Altinn fant ikke den forespurte ressursen. Kontroller system-ID og valgt miljø.",
                429 => "Altinn har begrenset antall kall. Vent litt før du prøver igjen.",
                >= 500 => "Altinn returnerte en serverfeil. Prøv igjen og kontroller tjenestens driftsstatus hvis feilen vedvarer.",
                _ => "Altinn avviste forespørselen. Kontroller feltene og rettighetene i forespørselen."
            });
            try
            {
                using var document = JsonDocument.Parse(altinn.ResponseBody);
                var upstream = document.RootElement;
                if (upstream.ValueKind == JsonValueKind.Object)
                {
                    if (upstream.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String) problem.Detail = detail.GetString();
                    if (upstream.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String) problem.Title = title.GetString();
                    if (upstream.TryGetProperty("errors", out var errors)) problem.Extensions["errors"] = errors.Clone();
                    if (upstream.TryGetProperty("validationErrors", out var validation)) problem.Extensions["validationErrors"] = validation.Clone();
                    if (upstream.TryGetProperty("code", out var code)) problem.Extensions["code"] = code.Clone();
                }
            }
            catch (JsonException) { /* Never send HTML or token-provider responses to the browser. */ }
        }
        if (exception is TokenRequestException tokenError)
            logger.LogWarning("Maskinporten token request failed: {Code}, {ProviderCode}, upstream HTTP {UpstreamStatus}, environment {Environment}, scope {Scope}, trace {TraceId}",
                tokenError.Code, tokenError.ProviderCode, tokenError.UpstreamStatus, tokenError.Environment, tokenError.Scope, context.HttpContext.TraceIdentifier);
        else
            logger.LogWarning("Upstream request failed with {ExceptionType}, HTTP {Status}, trace {TraceId}", exception.GetType().Name, status, context.HttpContext.TraceIdentifier);
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.Result = new ObjectResult(problem) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
