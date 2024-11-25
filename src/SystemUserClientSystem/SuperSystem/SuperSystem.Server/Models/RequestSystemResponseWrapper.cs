namespace smartcloud.server.Models
{
    public class RequestSystemResponseWrapper
    {
        public List<RequestSystemResponse> Data { get; set; }


        public Dictionary<string,string> Links { get; set;  }
    }
}
