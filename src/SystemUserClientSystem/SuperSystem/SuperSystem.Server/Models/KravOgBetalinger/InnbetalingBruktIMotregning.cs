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
    /// InnbetalingBruktIMotregning
    /// </summary>
    [DataContract]
        public partial class InnbetalingBruktIMotregning
    {
        /// <summary>
        /// Gets or Sets Innbetalingsidentifikator
        /// </summary>
        [DataMember(Name="innbetalingsidentifikator", EmitDefaultValue=false)]
        public string Innbetalingsidentifikator { get; set; }

        /// <summary>
        /// Gets or Sets UplassertBeloep
        /// </summary>
        [DataMember(Name="uplassertBeloep", EmitDefaultValue=false)]
        public double? UplassertBeloep { get; set; }
    }
}
