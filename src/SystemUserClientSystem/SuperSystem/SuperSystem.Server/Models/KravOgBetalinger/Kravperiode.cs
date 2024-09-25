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
    /// Kan være skattleggingsperiode; i forskjellige systemer dette er implementert som enten beskrivelse av periode + år (MVA - januar-februar 2022), eller som  dato interval - fraDato, tilDato i Særavgifter,eller  bare år som inntektsår for skatt, eller teknisk termin beskrivelse  (termin 1 2022) osv. I forsøk for å standardisere representasjon av skattleggingsperiode på tvers i form av periodeBeskrivelse + år ble introdusert felten periodeBeskrivelse som skal inneholde en standard tekst som beskriver best perioden (januar-februar, juni, inntektsår, osv.)
    /// </summary>
    [DataContract]
        public partial class Kravperiode
    {
       
        /// <summary>
        /// Gets or Sets PeriodeBeskrivelse
        /// </summary>
        [DataMember(Name="periodeBeskrivelse", EmitDefaultValue=false)]
        public MultiSpraakTekst PeriodeBeskrivelse { get; set; }

        /// <summary>
        /// Gets or Sets Aar
        /// </summary>
        [DataMember(Name="aar", EmitDefaultValue=false)]
        public string Aar { get; set; }

        /// <summary>
        /// Gets or Sets FraDato
        /// </summary>
        [DataMember(Name="fraDato", EmitDefaultValue=false)]
        public DateTime? FraDato { get; set; }

        /// <summary>
        /// Gets or Sets TilDato
        /// </summary>
        [DataMember(Name="tilDato", EmitDefaultValue=false)]
        public DateTime? TilDato { get; set; }

        /// <summary>
        /// Gets or Sets TerminTekniskNavn
        /// </summary>
        [DataMember(Name="terminTekniskNavn", EmitDefaultValue=false)]
        public string TerminTekniskNavn { get; set; }
      
    }
}
