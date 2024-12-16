using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Common.PEP.Constants;
using Altinn.Common.PEP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SystemUserApi.Clients.Interfaces;
using SystemUserApi.Models.Logistics;
using SystemUserApi.Models.Salary;

namespace SystemUserApi.Controllers
{

    /// <summary>
    /// This API exposes salary information for a given organization number
    /// It uses Altinn Authorization to check if the system user has the right to access the salary information
    /// 
    /// To be able to call Altinn Authorization PDP you need a subscription key from Altinn
    /// 
    /// You also need scope access to the API
    /// 
    /// Read more here https://docs.altinn.studio/authorization/guides/integrating-link-service/
    /// 
    /// The API is set up as a resource in Altinn Studio and deployed to resource registry. 
    /// Read more here. https://docs.altinn.studio/authorization/guides/create-resource-resource-admin/
    /// 
    /// The resource used in this example is called ttd_systembruker-salary and is deployed to TT02 environment
    /// https://platform.tt02.altinn.no/resourceregistry/api/v1/resource/ttd_systembruker-salary
    /// https://platform.tt02.altinn.no/resourceregistry/api/v1/resource/ttd_systembruker-salary/policy/
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {

        IAuthorization _authorization;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private MaskinportenConfig _maskinportenConfig;

        public SalaryController(IAuthorization authorization, IHttpContextAccessor httpContextAccessor, IOptions<MaskinportenConfig> maskinportenConfig)
        {
            _authorization = authorization;
            _httpContextAccessor = httpContextAccessor;
            _maskinportenConfig = maskinportenConfig.Value;
        }

        /// <summary>
        /// Endpoint for getting salary information for a given organization number
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{orgno}")]
        public async Task<IActionResult> GetSalaryInfo(string orgno)
        {
            // Create a XacmlJsonRequestRoot object with the request
            // Currently the PEP only supports one resource in the request so we need to clear the list and add the resource we want to check
            XacmlJsonRequestRoot xacmlJsonRequest = DecisionHelper.CreateDecisionRequest("adf", "ads", _httpContextAccessor.HttpContext.User, "read");
            xacmlJsonRequest.Request.Resource.Clear();
            xacmlJsonRequest.Request.Resource.Add(CreateResourceCategoryForResource("ttd_systembruker-salary", orgno, true));

            // Call the authorization service to check if the system user has the right to call this service
            XacmlJsonResponse xacmlJsonResponse = await _authorization.AuthorizeReqeust(xacmlJsonRequest);

            if (!xacmlJsonResponse.Response[0].Decision.Equals("Permit", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, "Access denied");
            }

            return Ok(new SalaryInfo() { SalaryStatus = "Paid", SalaryAmount = 1337 });
        }

        /// <summary>
        /// Builds resource category  
        /// </summary>
        /// <param name="resourceid"></param>
        /// <param name="organizationnumber"></param>
        /// <param name="includeResult"></param>
        /// <returns></returns>
        private XacmlJsonCategory CreateResourceCategoryForResource(string resourceid, string organizationnumber, bool includeResult = false)
        {
            XacmlJsonCategory resourceCategory = new XacmlJsonCategory();
            resourceCategory.Attribute = new List<XacmlJsonAttribute>();

            resourceCategory.Attribute.Add(DecisionHelper.CreateXacmlJsonAttribute(AltinnXacmlUrns.OrganizationNumberAttribute, organizationnumber, "http://www.w3.org/2001/XMLSchema#string", "Altinn", includeResult));
            resourceCategory.Attribute.Add(DecisionHelper.CreateXacmlJsonAttribute(AltinnXacmlUrns.ResourceId, resourceid, "http://www.w3.org/2001/XMLSchema#string", "Altinn"));

            return resourceCategory;
        }
    }
}
