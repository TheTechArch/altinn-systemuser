namespace SystemUserApi.Configuration
{
    /// <summary>
    /// Configuration to call the Altinn PDP endpoint
    /// </summary>
    public class AuthorizationConfig
    {
        /// <summary>
        /// The API Key required to call the PDP endpoint
        /// </summary>
        public required string ApiKey { get; set; }

        /// <summary>
        /// The PDP Endpoint in Altinn Platform
        /// </summary>
        public required string PDPEndpoint { get; set; }

        /// <summary>
        /// The scope required to call the PDP endpoint. Will be request in JWT Grant
        /// </summary>
        public required string PDPScope { get; set; }
    }
}
