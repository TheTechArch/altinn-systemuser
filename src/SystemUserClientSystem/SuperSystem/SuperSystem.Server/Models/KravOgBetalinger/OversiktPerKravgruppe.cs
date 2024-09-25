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
    /// oversikt over saldo, stipulerte renter og krav (forfallte og ikke forfalte) gruppert per kravgrupe ved forespørsels tidspunkt
    /// </summary>
    [DataContract]
        public partial class OversiktPerKravgruppe
    {
        /// <summary>
        /// Gets or Sets Kravgruppe
        /// </summary>
        [DataMember(Name="kravgruppe", EmitDefaultValue=false)]
        public string Kravgruppe { get; set; }

        /// <summary>
        /// Gets or Sets SumStipulerteRenter
        /// </summary>
        [DataMember(Name="sumStipulerteRenter", EmitDefaultValue=false)]
        public double? SumStipulerteRenter { get; set; }

        /// <summary>
        /// Gets or Sets SumForfalteKrav
        /// </summary>
        [DataMember(Name="sumForfalteKrav", EmitDefaultValue=false)]
        public double? SumForfalteKrav { get; set; }

        /// <summary>
        /// Gets or Sets SumIkkeForfalteKrav
        /// </summary>
        [DataMember(Name="sumIkkeForfalteKrav", EmitDefaultValue=false)]
        public double? SumIkkeForfalteKrav { get; set; }

        /// <summary>
        /// Gets or Sets Saldo
        /// </summary>
        [DataMember(Name="saldo", EmitDefaultValue=false)]
        public double? Saldo { get; set; }
    }
}
