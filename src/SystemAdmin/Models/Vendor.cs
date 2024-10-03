using System.Text.Json.Serialization;

namespace SystemAdmin.Models
{
    public class Vendor
    {

        /// <summary>
        /// The authority that defines organization numbers. 
        /// </summary>
        [JsonPropertyName("authority")]
        public string Authority => "iso6523-actorid-upis";

        [JsonPropertyName("ID")]
        public string ID { get; set; }
    }
}
