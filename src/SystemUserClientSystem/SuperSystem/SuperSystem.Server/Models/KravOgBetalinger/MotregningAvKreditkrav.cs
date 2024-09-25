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
    /// MotregningAvKreditkrav
    /// </summary>
    [DataContract]
        public partial class MotregningAvKreditkrav
    {
        /// <summary>
        /// Gets or Sets Motregningsidentifikator
        /// </summary>
        [DataMember(Name="motregningsidentifikator", EmitDefaultValue=false)]
        public string Motregningsidentifikator { get; set; }

        /// <summary>
        /// Gets or Sets Innkrevingsmyndighet
        /// </summary>
        [DataMember(Name="innkrevingsmyndighet", EmitDefaultValue=false)]
        public MultiSpraakTekst Innkrevingsmyndighet { get; set; }

        /// <summary>
        /// Gets or Sets Hovedkrav
        /// </summary>
        [DataMember(Name="hovedkrav", EmitDefaultValue=false)]
        public HovedKrav Hovedkrav { get; set; }

        /// <summary>
        /// kravet som blir dekket i motregningen; beskrivelse av dekket krav slik at det ser ut i disponeringsbrev; dekket krav her er ofte referert som motkrav i fagdomene.
        /// </summary>
        /// <value>kravet som blir dekket i motregningen; beskrivelse av dekket krav slik at det ser ut i disponeringsbrev; dekket krav her er ofte referert som motkrav i fagdomene.</value>
        [DataMember(Name="dekketkrav", EmitDefaultValue=false)]
        public List<DekketKrav> Dekketkrav { get; set; }

        /// <summary>
        /// Gets or Sets Motregnetdato
        /// </summary>
        [DataMember(Name="motregnetdato", EmitDefaultValue=false)]
        public DateTime? Motregnetdato { get; set; }
    }
}
