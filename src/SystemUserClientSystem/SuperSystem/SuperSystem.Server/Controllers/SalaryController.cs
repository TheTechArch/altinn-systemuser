using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartcloud.server.Clients;
using smartcloud.server.Clients.Interfaces;
using SystemUserApi.Models.Logistics;
using SystemUserApi.Models.Salary;

namespace smartcloud.server.Controllers
{

    /// <summary>
    /// This controller demonstrate how the end user system sofware creates a request to the a API endpoint. 
    /// 
    /// Depending on the rights of the system user, the API will return the salary information of the requested organization.
    /// 
    /// If the logged in organization does not have a system USer with the needed rights, the API will return a 403 Forbidden status code.
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ISalary _salaryClient;

        public SalaryController(ISalary salaryClient)
        {
            _salaryClient = salaryClient;
        }

        [HttpGet("{dataOrg}")]
        public async Task<IActionResult> Get(string dataOrg)
        {
            HttpContext.Request.Cookies.TryGetValue("smartcloudorg", out string? systemUserOrg);

            if (systemUserOrg == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Unauthorized. System user not logged in.");
            }

            SalaryInfo? status = await _salaryClient.GetSalaryInfo(systemUserOrg, dataOrg);

            if (status == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Forbidden. System user does not have the needed rights delegated to it.");
            }

            return Ok(status);
        }
    }
}
