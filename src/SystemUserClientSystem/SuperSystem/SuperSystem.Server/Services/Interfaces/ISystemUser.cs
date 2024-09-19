using SuperSystem.Server.Models;

namespace SuperSystem.Server.Services.Interfaces
{
    public interface ISystemUser
    {
        Task<CreateRequestSystemUserResponse> CreateSystemUserRequest(CreateRequestSystemUser registerSystemRequest);
    }
}
