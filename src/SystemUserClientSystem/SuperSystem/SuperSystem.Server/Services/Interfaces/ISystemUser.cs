﻿using smartcloud.server.Models;
using SmartCloud.Server.Models;

namespace SmartCloud.Server.Services.Interfaces
{
    /// <summary>
    /// SystemUser interface with integration functionality for Altinn
    /// </summary>
    public interface ISystemUser
    {
        /// <summary>
        /// Create a system user request based on a specific systemId and a organization
        /// Requires Maskinporten token with SystemRegister scope
        /// </summary>
        Task<CreateRequestSystemUserResponse> CreateSystemUserRequest(CreateRequestSystemUser registerSystemRequest, string token);

        /// <summary>
        /// Gets information about a specific request based on the requestId
        /// </summary>
        Task<CreateRequestSystemUserResponse> GetRequestStatus(string requestId, string token);    

        /// <summary>
        /// List all systemUsers for a system
        /// </summary>
        Task<List<SystemUser>> GetSystemUsersForSystem(string systemid, string token);

        /// <summary>
        /// List all request for a system
        /// </summary>
        Task<List<RequestSystemResponse>> GetRequestsForSystem(string systemId, string token);
    }
}
