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
        if (exception is MaskinportenConfigurationException) problem.Detail = exception.Message;
        if (exception is AltinnApiException altinn)
        {
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
        logger.LogWarning("Upstream request failed with {ExceptionType}, HTTP {Status}", exception.GetType().Name, status);
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.Result = new ObjectResult(problem) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
