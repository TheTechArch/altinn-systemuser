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
    /// tekst på spesifisert språk (&lt;a href&#x3D;&#x27;https://data.skatteetaten.no/begrep/20b2e244-9fe1-11e5-a9f8-e4115b280940&#x27;&gt;Begrepsreferanse&lt;/a&gt;)
    /// </summary>
    [DataContract]
        public partial class SpraakTekst
    {   
        /// <summary>
        /// Gets or Sets Tekst
        /// </summary>
        [DataMember(Name="tekst", EmitDefaultValue=false)]
        public string Tekst { get; set; }

        /// <summary>
        /// Gets or Sets Spraak
        /// </summary>
        [DataMember(Name="spraak", EmitDefaultValue=false)]
        public string Spraak { get; set; }
      
    }
}
