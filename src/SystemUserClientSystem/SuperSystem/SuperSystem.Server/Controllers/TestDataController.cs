using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SmartCloud.Server.Services;

namespace smartcloud.server.Controllers;

[ApiController]
[Route("api/testdata")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class TestDataController(TenorClient tenor) : ControllerBase
{
    [HttpGet("configuration")]
    public IActionResult Configuration() => Ok(new { enabled = tenor.Enabled });

    [HttpGet("organisations")]
    public async Task<IActionResult> Search([FromQuery, StringLength(100)] string? term, [FromQuery, StringLength(4)] string? organisationForm, CancellationToken ct)
    {
        try { return Ok(await tenor.Search(term, ct, organisationForm)); }
        catch (ValidationException error) { return Problem(detail: error.Message, statusCode: 400); }
    }

    [HttpGet("organisations/{organisationNumber}")]
    public async Task<IActionResult> Details([RegularExpression("^[0-9]{9}$")] string organisationNumber, CancellationToken ct)
    {
        var organisation = await tenor.Details(organisationNumber, ct);
        return organisation is null ? Problem(detail: "Testvirksomheten finnes ikke i Tenor.", statusCode: 404) : Ok(organisation);
    }
}
