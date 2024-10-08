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
namespace IO.Swagger.Model
{
    /// <summary>
    /// Periode
    /// </summary>
    [DataContract]
        public partial class Periode
    {
        /// <summary>
        /// Gets or Sets FraOgMed
        /// </summary>
        [DataMember(Name="fraOgMed", EmitDefaultValue=false)]
        public DateTime? FraOgMed { get; set; }

        /// <summary>
        /// Gets or Sets TilOgMed
        /// </summary>
        [DataMember(Name="tilOgMed", EmitDefaultValue=false)]
        public DateTime? TilOgMed { get; set; }
    }
}
