using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartCloud.Server.Models
{
    public class AttributePair
    {
        /// <summary>
        /// Gets or sets the attribute id for the match
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the attribute value for the match
        /// </summary>
        [Required]
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
