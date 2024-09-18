using Microsoft.AspNetCore.Mvc;

namespace SuperSystem.Server.Controllers
{
    [Route("[controller]")]
    public class RedirectController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("https://www.netflix.com");
        }
    }
}
