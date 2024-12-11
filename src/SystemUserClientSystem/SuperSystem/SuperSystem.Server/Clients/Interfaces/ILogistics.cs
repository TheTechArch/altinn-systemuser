using SystemUserApi.Models.Logistics;

namespace smartcloud.server.Clients.Interfaces
{
    public interface ILogistics
    {

        Task<LogisticStatus?> GetLogicstatus(string systemUserOrg, string dataOrganization);
    }
}
