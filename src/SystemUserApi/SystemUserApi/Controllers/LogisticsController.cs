using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Common.PEP.Constants;
using Altinn.Common.PEP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartCloud.Server.Config;
using SystemUserApi.Clients.Interfaces;

namespace SystemUserApi.Controllers
{
    /// <summary>
    /// This is an theoritical API to demonstrate Altinn SystemUser where this is a public API related to logistics
    /// 
    /// The system user calling this service would need rights depending to the operation called
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticsController : ControllerBase
    {

        IAuthorization _authorization;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private MaskinportenConfig _maskinportenConfig;

        public LogisticsController(IAuthorization authorization, IHttpContextAccessor httpContextAccessor, IOptions<MaskinportenConfig> maskinportenConfig)
        {
            _authorization = authorization;
            _httpContextAccessor = httpContextAccessor;
            _maskinportenConfig = maskinportenConfig.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{orgno}")]
        public async Task<IActionResult> GetLogisticsStatus(string orgno)
        {
            try
            {
                XacmlJsonRequestRoot xacmlJsonRequest = DecisionHelper.CreateDecisionRequest("adf", "ads", _httpContextAccessor.HttpContext.User, "read");
                xacmlJsonRequest.Request.Resource.Clear();
                xacmlJsonRequest.Request.Resource.Add(CreateResourceCategoryForResource("ttd_systembruker-logistikk-demo", orgno, true));

                // Call the authorization service to check if the system user has the right to call this service
                XacmlJsonResponse xacmlJsonResponse = await _authorization.AuthorizeReqeust(xacmlJsonRequest);


                if (!xacmlJsonResponse.Response[0].Decision.Equals("Permit",StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(JsonConvert.SerializeObject(xacmlJsonResponse));
                }

                return Ok("Logistics status");
            }catch(Exception ex)
            {
                return StatusCode(200, ex.Message + ex.StackTrace + "URLLLLLL:" + _maskinportenConfig.AltinnExchangeEndpoint);
            }
        }


        private XacmlJsonCategory CreateResourceCategoryForResource(string resourceid, string organizationnumber,bool includeResult = false)
        {
            XacmlJsonCategory resourceCategory = new XacmlJsonCategory();
            resourceCategory.Attribute = new List<XacmlJsonAttribute>();
            
            resourceCategory.Attribute.Add(DecisionHelper.CreateXacmlJsonAttribute(AltinnXacmlUrns.OrganizationNumberAttribute, organizationnumber, "http://www.w3.org/2001/XMLSchema#string", "Altinn", includeResult));
            resourceCategory.Attribute.Add(DecisionHelper.CreateXacmlJsonAttribute(AltinnXacmlUrns.ResourceId, resourceid, "http://www.w3.org/2001/XMLSchema#string", "Altinn"));
            
            return resourceCategory;
        }

    }
}
