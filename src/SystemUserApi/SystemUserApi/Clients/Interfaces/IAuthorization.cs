using Altinn.Authorization.ABAC.Xacml.JsonProfile;

namespace SystemUserApi.Clients.Interfaces
{
    public interface IAuthorization
    {
        Task<XacmlJsonResponse> AuthorizeReqeust(XacmlJsonRequestRoot xacmlJsonRequestRoot);
    }
}
