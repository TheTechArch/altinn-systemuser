using System.Threading.Tasks;
using Altinn.SystemAdmin.Maskinporten.Config;
using Altinn.SystemAdmin.Maskinporten.Models;

namespace Altinn.SystemAdmin.Maskinporten.Interfaces
{
    public interface IClientDefinition
    {
        MaskinportenConfig ClientSettings { get; set; }
        Task<ClientSecrets> GetClientSecrets();
    }

    public interface IClientDefinition<T> : IClientDefinition where T : IClientDefinition { }
}
