using SmartCloud.Server.Models;

namespace smartcloud.server.Models
{
    public class SystemUserWrapper
    {
        public List<SystemUser> Data { get; set; }


        public Dictionary<string,string> Links { get; set;  }
    }
}
