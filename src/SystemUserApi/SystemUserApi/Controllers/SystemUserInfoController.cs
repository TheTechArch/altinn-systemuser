using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SystemUserApi.Models;
using SystemUserApi.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUserInfoController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemUserInfoController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }

        /// <summary>
        /// Demonstrates how to get the system user from the context
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public SystemUserClaim? Get()
        {
            SystemUserClaim? systemUser = SystemUserUtil.GetSystemUser(_httpContextAccessor.HttpContext);
            return systemUser;
        }
    }
}
