using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace SmartCloud.Server.Controllers;

[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class RedirectController : Controller
{
    // Existing onboarding links now open the configurable form. Creating a request requires POST.
    public IActionResult Index([FromQuery] string systemUserOrg, [FromQuery] string? product) =>
        Redirect(QueryHelpers.AddQueryString("/vendor/new", new Dictionary<string, string?>
        {
            ["org"] = systemUserOrg,
            ["product"] = product
        }));
}
