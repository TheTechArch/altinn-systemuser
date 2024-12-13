using SystemUserApi.Models.Salary;

namespace smartcloud.server.Clients.Interfaces
{
    public interface ISalary
    {
        Task<SalaryInfo?> GetSalaryInfo(string systemUserOrg, string dataOrg);
    }
}
