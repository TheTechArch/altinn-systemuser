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
        public string? Client { get; set;  }

        /// <summary>
        /// The consumer
        /// </summary>
        public string? Consumer { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public string? SystemUserRequestScope { get; set; }
    }
}
