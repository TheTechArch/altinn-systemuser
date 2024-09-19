using Microsoft.AspNetCore.Mvc;

namespace SuperSystem.Server.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("https://www.netflix.com");
        }
    }
}
