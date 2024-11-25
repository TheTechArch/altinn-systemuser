namespace SmartCloud.Server.Config
{
    public class SystemRegisterConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string? BaseAdress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string SystemRegisterScope { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public required string RequestSystemUserScope { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public required string ScopeSystemUserRequestRead { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RegisterSystemEndpoint { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public string? SystemInfoPath { get; set;  }

        /// <summary>
        /// The path to request a systemUser 
        /// </summary>
        public string RequestSystemUserPath { get; set;  } = "systemuser/request/vendor";

        /// <summary>
        /// The path to list systemUsers for a system
        /// </summary>
        public string SystemUserListForSystemPath { get; set; } = "systemuser/vendor/bysystem/";

        /// <summary>
        /// The path to list request for a system
        /// </summary>
        public string RequestSystemPath { get; set; } = "systemuser/request/vendor/bysystem/";

        /// <summary>
        /// SystemId 
        /// </summary>
        public required string SystemId { get; set; }

        /// <summary>
        /// Comma seperated string identifying all Altinn resources that the system should have access to
        /// </summary>
        public required string RightResources { get; set; }
    }
}
