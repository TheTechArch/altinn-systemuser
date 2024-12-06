namespace SmartCloud.Server.Config
{
    public class MaskinportenConfig
    {
        /// <summary>
        /// The clientId
        /// </summary>
        public required string ClientId { get; set;  }

        /// <summary>
        /// Base 64 Encoded JWK
        /// </summary>
        public required string EncodedJwk { get; set;  }
        
        /// <summary>
        /// Set Maskinporten environment
        /// </summary>
        public required string Environment { get; set; } = "test";

        /// <summary>
        /// Exhange endpoint
        /// </summary>
        public required string AltinnExchangeEndpoint { get; set; }
    }
}
