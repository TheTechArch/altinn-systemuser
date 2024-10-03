using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Altinn.SystemAdmin.Maskinporten.Models
{
    public class ClientSecrets
    {
        public JsonWebKey ClientKey { get; set; }

        public X509Certificate2 ClientCertificate { get; set; }
    }
}
