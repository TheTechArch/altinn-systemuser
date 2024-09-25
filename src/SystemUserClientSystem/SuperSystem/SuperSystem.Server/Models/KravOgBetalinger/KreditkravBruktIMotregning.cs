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
    /// KreditkravBruktIMotregning
    /// </summary>
    [DataContract]
        public partial class KreditkravBruktIMotregning
    {
        
        /// <summary>
        /// Gets or Sets KravforfallsIdentifikator
        /// </summary>
        [DataMember(Name="kravforfallsIdentifikator", EmitDefaultValue=false)]
        public string KravforfallsIdentifikator { get; set; }

        /// <summary>
        /// Gets or Sets GjenstaaendeBeloep
        /// </summary>
        [DataMember(Name="gjenstaaendeBeloep", EmitDefaultValue=false)]
        public double? GjenstaaendeBeloep { get; set; }

        /// <summary>
        /// Gets or Sets Kravbeskrivelse
        /// </summary>
        [DataMember(Name="kravbeskrivelse", EmitDefaultValue=false)]
        public MultiSpraakTekst Kravbeskrivelse { get; set; }

        /// <summary>
        /// Gets or Sets Kravperiode
        /// </summary>
        [DataMember(Name="kravperiode", EmitDefaultValue=false)]
        public Kravperiode Kravperiode { get; set; }
    }
}
