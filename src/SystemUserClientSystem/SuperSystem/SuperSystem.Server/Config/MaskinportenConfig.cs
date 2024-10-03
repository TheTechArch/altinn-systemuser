namespace SuperSystem.Server.Config
{
    public class MaskinportenConfig
    {
        /// <summary>
        /// The base64 encoded certificate
        /// </summary>
        public string? Cert { get; set;  }

        /// <summary>
        /// The clientId
        /// </summary>
        public required string ClientId { get; set;  }

        /// <summary>
        /// The consumer
        /// </summary>
        public string? Consumer { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public string? SystemUserRequestScope { get; set; }

        /// <summary>
        /// The KeyID for the maskinporten client
        /// </summary>
        public string? KeyId { get; set; }

        /// <summary>
        /// Scope define by Altinn to be alloed to create system user requests
        /// </summary>
        public string? RequestSystemUserScope { get; set; }

        /// <summary>
        /// Scope for maskinporten client when calling API as system user. This will vary depending on the API the system user need to call
        /// </summary>
        public string? SystemUserScope { get; set;  }

        /// <summary>
        /// Base 64 Encoded JWK
        /// </summary>
        public required string EncodedJwk { get; set;  }
    }
}
