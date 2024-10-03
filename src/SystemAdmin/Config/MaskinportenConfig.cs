using Altinn.SystemAdmin.Maskinporten.Interfaces;

namespace Altinn.SystemAdmin.Maskinporten.Config
{
    public class MaskinportenConfig
    {
        /// <summary>
        /// ClientID if fixed
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Scope if fixed
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// The Maskinporten environment. Valid values are ver1, ver2 or prod
        /// </summary>
        public string? Environment { get; set; }

        /// <summary>
        /// Base64 Encoded Json Web Key 
        /// </summary>
        public string? EncodedJwk { get; set; }
    }

    public class MaskinportenSettings<T> : MaskinportenConfig where T : IClientDefinition { }
}
