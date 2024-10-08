/* 
 * Krav og betalinger API
 *
 * Tjeneste som tilbyr oversikt av krav, innbetalinger og utbetalinger tilhørende en part.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace IO.Swagger.Model
{
    /// <summary>
    /// Defines Utbetalingstatus
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Utbetalingstatus
    {
        /// <summary>
        /// Enum Bekreftet for value: bekreftet
        /// </summary>
        [EnumMember(Value = "bekreftet")]
        Bekreftet = 1,
        /// <summary>
        /// Enum UnderUtbetaling for value: underUtbetaling
        /// </summary>
        [EnumMember(Value = "underUtbetaling")]
        UnderUtbetaling = 2    }
}
