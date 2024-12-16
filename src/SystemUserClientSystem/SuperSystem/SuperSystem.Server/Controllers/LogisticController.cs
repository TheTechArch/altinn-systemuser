using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smartcloud.server.Clients.Interfaces;
using SystemUserApi.Models.Logistics;

namespace smartcloud.server.Controllers
{


    /// <summary>
    /// API supporting Logistc features in SmartCloud.
    /// 
    /// In a real-world scenario this would be the place to implement the business logic for the Logistic features.
    /// 
    /// In this example, the controller demonstrates how a client can be denied access when system user does not have the needed rights delegated to it.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticController : ControllerBase
    {
        
        ILogistics _logisticsClient;

        public LogisticController(ILogistics logisitcs) 
        {
            _logisticsClient = logisitcs;
        }

        
        [HttpGet("{dataOrg}")]
        public async Task<IActionResult> Get(string dataOrg)
        {
            HttpContext.Request.Cookies.TryGetValue("smartcloudorg", out string? systemUserOrg);

            if(systemUserOrg == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Unauthorized. System user not logged in.");
            }

            LogisticStatus? status = await _logisticsClient.GetLogicstatus(systemUserOrg, dataOrg);

            if(status == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Forbidden. System user does not have the needed rights delegated to it.");
            }



            return Ok(status);
        }
    }
}
