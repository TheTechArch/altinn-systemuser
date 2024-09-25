/* 
 * Krav og betalinger API
 *
 * Tjeneste som tilbyr oversikt av krav, innbetalinger og utbetalinger tilhørende en part.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
namespace IO.Swagger.Model
{
    /// <summary>
    /// Bankkonto
    /// </summary>
    [DataContract]
        public partial class Bankkonto
    {
        /// <summary>
        /// Gets or Sets Bankinformasjon
        /// </summary>
        [DataMember(Name="bankinformasjon", EmitDefaultValue=false)]
        public string Bankinformasjon { get; set; }

        /// <summary>
        /// Gets or Sets KontoeiersNavn
        /// </summary>
        [DataMember(Name="kontoeiersNavn", EmitDefaultValue=false)]
        public string KontoeiersNavn { get; set; }

        /// <summary>
        /// Gets or Sets Kontonummer
        /// </summary>
        [DataMember(Name="kontonummer", EmitDefaultValue=false)]
        public string Kontonummer { get; set; }

        /// <summary>
        /// Gets or Sets Iban
        /// </summary>
        [DataMember(Name="iban", EmitDefaultValue=false)]
        public string Iban { get; set; }

        /// <summary>
        /// Gets or Sets SwiftBIC
        /// </summary>
        [DataMember(Name="swiftBIC", EmitDefaultValue=false)]
        public string SwiftBIC { get; set; }

    }
}
