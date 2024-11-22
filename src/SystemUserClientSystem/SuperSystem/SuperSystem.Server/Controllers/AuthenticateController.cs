using Microsoft.AspNetCore.Mvc;

namespace smartcloud.server.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthenticateController : Controller
    {
        /// <summary>
        /// Login the organization with the given system user organization number
        /// In a real world application the applicaiton wouild need to loging the end user and authorize him for access to the organization and its system user
        /// This would 
        /// </summary>
        /// <param name="systemUserOrg"></param>
        /// <returns></returns>
        public IActionResult Index([FromQuery] string systemUserOrg)
        {
            // Create a new cookie with the name "smartcloudorg" and the value of the systemUserOrg parameter
            Response.Cookies.Append("smartcloudorg", systemUserOrg);

            return Redirect("/dashboard");
        }
    }
}
